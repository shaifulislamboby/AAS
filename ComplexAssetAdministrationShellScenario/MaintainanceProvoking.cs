using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ComplexAssetAdministrationShellScenario.Serializers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ComplexAssetAdministrationShellScenario
{
    public static class OrderStatus
    {
        public static string OrderAccepted = "OrderAccepted";
        public static string AASOrderCompletionNotification = "AASOrderCompletionNotification";
        public static string OrderCompleted = "OrderCompleted";
    }

    public class MaintenanceProvoking
    {
        private static string _conversationId;
        private static dynamic _data;
        private static RootObject rootObject;
        private static Dictionary<string, object> dataDictionary;
        private static string messageId;
        private static string machineName;
        private static string maintenanceThreshold;
        private static string sender;
        
        public static async Task call_maintenance_endpoint(string jsonData, DataStorage storage, string realRoot)
        {
            try
            {
                _data = JsonConvert.DeserializeObject(jsonData);
                if (_data.interactionElements[0]["embeddedDataSpecifications"] != null &&
                    _data.interactionElements[0]["dataSpecification"] != null)
                {
                    _conversationId = _data.frame.conversationId;
                    messageId = _data.frame.messageId;
                    machineName = _data.interactionElements[0].value.MaintenanceElement.value;
                    maintenanceThreshold = _data.interactionElements[0].value.MaintenanceThreshold.value;
                    sender = _data.frame.sender.identification.id;

                }
                else
                {
                    _conversationId = _data.frame.conversationId;
                    messageId = _data.frame.messageId;
                    machineName = _data.interactionElements[0].value[0].value;
                    maintenanceThreshold = _data.interactionElements[0].value[1].value;
                    sender = _data.frame.sender.identification.id;
                }
                
                dataDictionary = new Dictionary<string, object>();

                if (_data.frame.type == "RESPOND")
                {
                    if (!storage.dataDictionary.ContainsKey(_conversationId))
                    {
                        var data = storage.dataDictionary[_conversationId];
                        bool value = data.ContainsKey("OrderStatus");
                        if (value == true && data["OrderStatus"] == OrderStatus.AASOrderCompletionNotification)
                        {
                            data["OrderStatus"] = OrderStatus.OrderCompleted;
                        }
                        
                        
                    }
                    else
                    {
                        HandleNotifyInit();
                    }
                    
                }
                if(_data.frame.type == "NOTIFY_INIT")
                {  if (storage.dataDictionary.ContainsKey(_conversationId))
                    {
                       var data = storage.dataDictionary[_conversationId];
                       bool value = data.ContainsKey("OrderStatus");
                       if (value == true && data["OrderStatus"] == OrderStatus.OrderAccepted)
                       {
                           //
                       }
                       else
                       {
                           
                       }
                    }
                    else
                    {   try{
                        HandleNotifyInit();
                        }
                        catch (Exception ex)
                        {
                        Console.WriteLine($"Error occurred during the POST request: {ex.Message}");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during the POST request: {ex.Message}");
            }

            async void HandleNotifyInit()
            {
               
                string url =Program.Configuration["MES_APPLICATION_CONFIG:MES_ENDPOINT"];
                Console.WriteLine(url);
                string Data =
                    $"{{ \"ConversationId\": \"{_conversationId}\", \"MessageId\": \"{messageId}\", \"MachineName\": \"{machineName}\", \"MaintenanceThreshold\": {maintenanceThreshold} }}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync(url,
                        new StringContent(Data, Encoding.UTF8, "application/json"));
                    Console.WriteLine(response);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    { 
                        // Order accepted
                        // --------- step oen : empty storage, request, 1, mes-aas --> MES , success, save data with accepted status
                        dataDictionary.Add("data", _data);
                        dataDictionary.Add("messageId", messageId);
                        dataDictionary.Add("maintenanceThreshold", maintenanceThreshold);
                        dataDictionary.Add("machineName", machineName);
                        dataDictionary.Add("OrderStatus", OrderStatus.OrderAccepted);
                        dataDictionary.Add("receiver", _data.frame.sender.identification.id);
                        // data saving point, initial point
                        storage.SaveData(_conversationId, dataDictionary);
                        
                        Console.WriteLine("POST request successful!");
                        string responseContent = await response.Content.ReadAsStringAsync();
                        string outBoundMessageString = String.Empty;
                        try
                        {
                            MaintenanceSerializer maintenanceData = JsonConvert.DeserializeObject<MaintenanceSerializer>(responseContent);
                            MaintenanceActions.MaintenanceActionsInitialization(url = Program.Configuration["MES_APPLICATION_CONFIG:MES_AAS_ENDPOINT"]);
                            MaintenanceActions.UpdateMaintenanceRecord(maintenanceData);
                            storage.ModifyInnerValue(maintenanceData.ConversationId, "messageId",
                                maintenanceData.MessageId);
                            storage.ModifyInnerValue(maintenanceData.ConversationId, "OrderStatus",
                                OrderStatus.OrderAccepted);
                            Console.WriteLine(maintenanceData.MessageId);
                            I40Message outBoundMessage = new I40Message();
                            var mt = MaintenanceType.GetMaintenanceType(int.Parse(maintenanceThreshold));
                            var ie = MaintenanceActions.GetMaintenanceRecord(machineName, mt);

                            var frame = FrameBuilder.CreateFrame(receiver: sender,
                                conversationId: maintenanceData.ConversationId, messageId: maintenanceData.MessageId,
                                messageType: "NOTIFY_ACCEPTED");
                            outBoundMessage.frame = frame;
                            outBoundMessage.interactionElements = ie;
                            outBoundMessageString = JsonConvert.SerializeObject(outBoundMessage);
                            Console.WriteLine(outBoundMessageString);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        
                        try
                        {
                            _ = Task.Run(() =>
                            {
                                MqttPublisherAndReceiver.MqttPublishAsync(MqttPublisherAndReceiver.brockerAddress,
                                    MqttPublisherAndReceiver.brockerPort,Program.Configuration["MES_APPLICATION_CONFIG:PUBLICATION_TOPIC"] ,outBoundMessageString);
                                return Task.CompletedTask;
                            });
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("publishing got following exceptions: " + e);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"POST request failed with status code: {response.StatusCode}");
                    }
                }
            }
        }
    }
}