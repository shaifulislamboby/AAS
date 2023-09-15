using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ComplexAssetAdministrationShellScenario.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ComplexAssetAdministrationShellScenario
{
    public static class OrderStatus
    {
        public static string OrderReceived = "OrderReceived";
        public static string OrderAccepted = "OrderAccepted";
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

                if (_data.frame.messageType == "RESPOND")
                {
                    //
                }
                else
                {
                    HandleNotifyInit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during the POST request: {ex.Message}");
            }

            async void HandleNotifyInit()
            {
                dataDictionary.Add("data", _data);
                dataDictionary.Add("messageId", messageId);
                dataDictionary.Add("maintenanceThreshold", maintenanceThreshold);
                dataDictionary.Add("machineName", machineName);
                dataDictionary.Add("OrderStatus", OrderStatus.OrderReceived);
                dataDictionary.Add("receiver", _data.frame.sender.identification.id);
                storage.SaveData(_conversationId, dataDictionary);
                string url = "https://da80-2003-c4-bf08-e590-8d85-1a78-c5a4-e998.ngrok.io/eai/EAI_BaSys_L?MACHINE_ID=4"; // Replace with the actual API endpoint URL
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
                        Console.WriteLine("POST request successful!");
                        string responseContent = await response.Content.ReadAsStringAsync();
                        string outBoundMessageString = String.Empty;
                        try
                        {
                            MaintenanceSerializer maintenanceData = JsonConvert.DeserializeObject<MaintenanceSerializer>(responseContent);
                            // todo: here Rafiul will this line so that we can dynamically update url
                            MaintenanceActions.MaintenanceActionsInitialization(url = "http://localhost:5111");
                            MaintenanceActions.UpdateMaintenanceRecord(maintenanceData);
                            storage.ModifyInnerValue(maintenanceData.ConversationId, "messageId",
                                maintenanceData.MessageId);
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
                                    MqttPublisherAndReceiver.brockerPort, "aas-notification",outBoundMessageString);
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