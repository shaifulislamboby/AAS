using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
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
        public int MaintenanceDuration { get; set; }
        public string? MaintenanceStaff { get; set; }
        public double? MaintenanceCosts { get; set; }
    }

    public class MesAasPostHandler
    {
        public async Task HandlePostRequest(HttpContext context, DataStorage dataStorage)
        {
            try
            {
                // Read the request body
                string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                /* data --->>>  
                /* data --->>>  
                 * {
        "conversationId": "DMU80eVo1_500_Maintenance1::1",        
        "MessageId": "3",
        "MachineName": "DMU80eVo1",
        "MaintenanceThreshold": 500,
        "ActualMaintenanceStart": "2023-04-29T14:17:49.13",
        "ActualMaintenanceEnd": "2023-04-30T08:10:21.13",
	     "MaintenanceDuration": 3600,
	     "MaintenanceStaff": "",
	     "MaintenanceCost: 222,
	     "":
}
                 */

                // Check if the request body is not null or empty
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
                MaintenanceActions.MaintenanceActionsInitialization("http://localhost:5111");
                // this line is updating the value in the MES-AAS server. 
                MaintenanceActions.UpdateMaintenanceRecordForCompletionMessage(requestData);
                var mt = MaintenanceType.GetMaintenanceType(requestData.MaintenanceThreshold);
                var ie = MaintenanceActions.GetMaintenanceRecord(requestData.MachineName, mt);
                var rDictionary = dataStorage.dataDictionary[requestData.ConversationId];
                object receiver = null;
                if (rDictionary.TryGetValue("receiver", out var value))
                {
                    receiver = value;
                    // Now 'value' contains the value associated with 'innerKey' in the 'outerKey' dictionary.
                }

                object data = rDictionary["data"];
                I40Message outBoundMessage = new I40Message();
                

                var frame = FrameBuilder.CreateFrame(receiver: receiver.ToString(), conversationId: requestData.ConversationId, messageId: Int32.Parse(requestData.MessageId),
                    messageType: "CHANGE");
                outBoundMessage.frame = frame;
                outBoundMessage.interactionElements = ie;
                string outBoundMessageString = JsonConvert.SerializeObject(outBoundMessage);
                //var publishingChange = RootObjectBuilder.CreateRootJson(interactionElement: ie,
                  //  conversationId: requestData.ConversationId, messageId: int.Parse(requestData.MessageId),
                    //receiver: receiver.ToString(), messageType: "change");
                try
                {
                    _ = Task.Run(() =>
                    {
                        MqttPublisherAndReceiver.MqttPublishAsync(MqttPublisherAndReceiver.brockerAddress,
                            MqttPublisherAndReceiver.brockerPort, "aas-notification", outBoundMessageString);
                        return Task.CompletedTask;
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine("publishing got following exceptions: " + e);
                }

                // Serialize the processed data back to JSON
                string responseData = JsonSerializer.Serialize(requestData);

                // Set the response content type to application/json
                context.Response.ContentType = "application/json";

                // Set the response status code to 200 OK
                context.Response.StatusCode = StatusCodes.Status200OK;

                // Write the JSON data as the response body
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