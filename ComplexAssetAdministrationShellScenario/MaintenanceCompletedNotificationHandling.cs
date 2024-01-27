using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ComplexAssetAdministrationShellScenario.Serializers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ComplexAssetAdministrationShellScenario
{
    public class MaintenanceData
    {
        public string ConversationId { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public int MaintenanceThreshold { get; set; }
        public DateTime ActualMaintenanceStart { get; set; }
        public DateTime ActualMaintenanceEnd { get; set; }
        public DateTime PlannedMaintenanceStart { get; set; }
        public DateTime PlannedMaintenanceEnd { get; set; }
        /*
        {
    [3:35 PM] Luan Salaj
{
    "MessageId":"3",
    "MachineName":"0",
    "MaintenanceThreshold":500,
    "ActualMaintenanceStart":"2023-12-11T15:31:01",
    "ActualMaintenanceEnd":"2023-12-11T15:33:44",
    "PlannedMaintenanceStart":"2024-02-05T01:47:11",
    "PlannedMaintenanceEnd":"2024-02-05T02:02:11",
    "MaintenanceDuration":164,
    "MaintenanceStaff":"Zimmermann Fabian",
    "MaintenanceCosts":0.0,
    "ConversationId":"Maintenance_1::503"
}
}
         
         */
        public int MaintenanceDuration { get; set; }
        public string? MaintenanceStaff { get; set; }
        public double? MaintenanceCosts { get; set; }
    }

    public class MesAasPostHandler
    {
        private async Task RetryPolicy(string retryMessage, string conversationId, DataStorage dataStorage)
        {
            Console.WriteLine("RetryPolicy started");

            for (int retry = 0; retry < 5; retry++)
            {
                await Task.Delay(5000);
                var rDictionary = dataStorage.dataDictionary[conversationId];


                if (rDictionary["OrderStatus"] == OrderStatus.AASOrderCompletionNotification)
                {
                    try
                    {
                        _ = Task.Run(() =>
                        {
                            MqttPublisherAndReceiver.MqttPublishAsync(MqttPublisherAndReceiver.brockerAddress,
                                MqttPublisherAndReceiver.brockerPort,
                                Program.Configuration["MES_APPLICATION_CONFIG:PUBLICATION_TOPIC"], retryMessage);
                            return Task.CompletedTask;
                        });

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("publishing got following exceptions: " + e);
                    }

                }

               else if (rDictionary["OrderStatus"] == OrderStatus.OrderCompleted)
                    
                {
                    Console.WriteLine("We are stopping the retry policy for change message, conversation id: " + conversationId);
                    break;
                    

                }
                
            }
        }

        public async Task HandlePostRequest(HttpContext context, DataStorage dataStorage)
        {
            try
            {
                // Read the request body
                string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                
                if (string.IsNullOrEmpty(requestBody))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Request body is null or empty.");
                    return;
                }

                // Deserialize the JSON data to a C# object
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, };
                var requestData = JsonSerializer.Deserialize<MaintenanceData>(requestBody, options);

                // Check for null or empty strings in the request data
                if (string.IsNullOrEmpty(requestData.ConversationId) || string.IsNullOrEmpty(requestData.MessageId) ||
                    string.IsNullOrEmpty(requestData.MachineName))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(
                        "ConversationId, MessageId, and MachineName cannot be null or empty.");
                    return;
                }

                // Process the request data (add your custom logic here)
                // For example, you can access individual properties like:
                string conversationId = requestData.ConversationId;
                string messageId = requestData.MessageId;
                //
                MaintenanceActions.MaintenanceActionsInitialization(Program.Configuration["MES_APPLICATION_CONFIG:MES_AAS_ENDPOINT"]);
                // this line is updating the value in the MES-AAS server. 
                
                MaintenanceActions.UpdateMaintenanceRecordForCompletionMessage(requestData);
                var mt = MaintenanceType.GetMaintenanceType(requestData.MaintenanceThreshold);
                var ie = MaintenanceActions.GetMaintenanceRecord(requestData.MachineName, mt);
                try
                {
                    var rDictionary = dataStorage.dataDictionary[requestData.ConversationId];
                    object receiver = null;
                if (rDictionary.TryGetValue("receiver", out var value))
                {
                    receiver = value;
                    // Now 'value' contains the value associated with 'innerKey' in the 'outerKey' dictionary.
                }

                object data = rDictionary["data"];
                I40Message outBoundMessage = new I40Message();
                

                var frame = FrameBuilder.CreateFrame(receiver: receiver.ToString(), conversationId: requestData.ConversationId, messageId: 1 + Int32.Parse(requestData.MessageId),
                    messageType: "CHANGE");
                outBoundMessage.frame = frame;
                outBoundMessage.interactionElements = ie;
                string outBoundMessageString = JsonConvert.SerializeObject(outBoundMessage);
                try
                {
                    _ = Task.Run(() =>
                    {
                        MqttPublisherAndReceiver.MqttPublishAsync(MqttPublisherAndReceiver.brockerAddress,
                            MqttPublisherAndReceiver.brockerPort, Program.Configuration["MES_APPLICATION_CONFIG:PUBLICATION_TOPIC"], outBoundMessageString);
                        return Task.CompletedTask;
                    });
                    rDictionary["OrderStatus"] = OrderStatus.AASOrderCompletionNotification;
                    RetryPolicy(outBoundMessageString, conversationId, dataStorage);

                }
                catch (Exception e)
                {
                    Console.WriteLine("publishing got following exceptions: " + e);
                }
                Console.WriteLine(outBoundMessageString);

                // Serialize the processed data back to JSON
                

                // Set the response content type to application/json
                context.Response.ContentType = "application/json";

                // Set the response status code to 200 OK
                context.Response.StatusCode = StatusCodes.Status200OK;

                // Write the JSON data as the response body
               
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                string responseData = JsonSerializer.Serialize(requestData);
                await context.Response.WriteAsync(
                    "Your POST request has been processed successfully and we received this data from your side: " +
                    responseData);
               
                
            }
            catch (JsonException)
            {
                // Handling JSON parse errors
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid JSON data in the request body.");
            }
            catch (Exception ex)
            {
                // Handling other exceptions
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync($"An error occurred: {ex.Message}");
            }
        }
    }
}