using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
                rootObject = JsonConvert.DeserializeObject<RootObject>(jsonData);
                dataDictionary = new Dictionary<string, object>();
                _conversationId = rootObject.Frame.ConversationId;
                messageId = _data.frame.messageId;
                machineName = _data.interactionElements[0].value[0].value;
                maintenanceThreshold = _data.interactionElements[0].value[1].value;
                sender = rootObject.Frame.Sender.Identification.Id;
                if (GetMessageType(rootObject).Contains("respond"))
                {
                    //
                }
                else
                {
                    HandleNotifyInit();
                }

                /* 
                 * {"interactionElements":
                 * [{"idShort":"MaintenanceOrderDescription",
                 * "modelType":{"name":"SubmodelElementCollection"},
                 * "value":[{"idShort":"MaintenanceElement","modelType":{"name":"Property"},
                 * "value":"DMU80eVo1","valueType":"string"},
                 * {"idShort":"MaintenanceThreshold",
                 * "modelType":{"name":"Property"},
                 * "value":250,"valueType":"int"},
                 * {"idShort":"MaintenaceCompany",
                 * "modelType":{"name":"Property"},
                 * "value":"Lauscher","valueType":"string"},
                 * {"idShort":"MaintenanceCompanyLocation",
                 * "modelType":{"name":"Property"},
                 * "value":"Achen","valueType":"string"}]}],
                 *
                 * "frame":{"type":"notify_init"/"", ---->>> this type will change(as per Rafiul's dictation)
                 * "sender":{"identification":{"id":"BASYX_MACHINE_AAS_1","idType":"CUSTOM"},
                 * "role":{"name":"InformationSender"}},
                 * "receiver":{"identification":{"id":"MES_AAS","idType":"CUSTOM"},
                 * "role":{"name":"informationReceiver"}},"conversationId":null,
                 * "messageId":1,"inReplyTO":null,"replyBy":null,
                 * "semanticProtocol":{"keys":[{"type":"GlobalReference","idType":"CUSTOM","value":"Maintenance"}]}}}
                 */
                /* when this function receive notify init then after deserializing the message,
                 first check if there is an entry in the data dict with this conversationId,
                 if not then make a entry and if available then modify 
                 {"conversationId": 
                 {"data": notify_init_full_data,
                 "messageId": messageId,
                 "machineName": machineName,
                 "maintenanceThreshold": maintenanceThreshold,
                 "OrderStatus": OrderStatus}
                 }
                 */
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during the POST request: {ex.Message}");
            }

            async void HandleNotifyInit()
            {
                dataDictionary.Add("data", rootObject);
                dataDictionary.Add("messageId", messageId);
                dataDictionary.Add("maintenanceThreshold", maintenanceThreshold);
                dataDictionary.Add("machineName", machineName);
                dataDictionary.Add("OrderStatus", OrderStatus.OrderReceived);
                dataDictionary.Add("receiver", rootObject.Frame.Sender.Identification.Id);
                storage.SaveData(_conversationId, dataDictionary);
                string url = "http://127.0.0.1:8000/maintenance-request/"; // Replace with the actual API endpoint URL

                // Create a JSON string containing the arguments as data
                string Data =
                    $"{{ \"conversationId\": \"{_conversationId}\", \"MessageId\": \"{messageId}\", \"MachineName\": \"{machineName}\", \"MaintenanceThreshold\": {maintenanceThreshold} }}";

                // Create a new HttpClient instance
                using (HttpClient client = new HttpClient())
                {
                    // Make the POST request
                    HttpResponseMessage response = await client.PostAsync(url,
                        new StringContent(Data, Encoding.UTF8, "application/json"));
                    Console.WriteLine(response);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("POST request successful!");
                        string responseContent = await response.Content.ReadAsStringAsync();
                        JObject jsonResponse = JObject.Parse(responseContent);
                        JObject successObject = jsonResponse["success"] as JObject;

                        // Deserialize the successObject into a MaintenanceSerializer instance
                        MaintenanceSerializer maintenanceData = successObject.ToObject<MaintenanceSerializer>();
                        // todo: here Rafiul will this line so that we can dynamically update url
                        MaintenanceActions.MaintenanceActionsInitialization(url = "http://localhost:5111");
                        // this line is updating the value in the MES-AAS server. 
                        MaintenanceActions.UpdateMaintenanceRecord(maintenanceData);
                        storage.ModifyInnerValue(maintenanceData.ConversationId, "messageId",
                            maintenanceData.MessageId);

                        // Now you can work with the parsed object
                        Console.WriteLine(maintenanceData.MessageId);
                        var mt = MaintenanceType.GetMaintenanceType(int.Parse(maintenanceThreshold));
                        var ie = MaintenanceActions.GetMaintenanceRecord(machineName, mt);
                        List<string> iel = new List<string> { ie };
                        realRoot = RootObjectBuilder.CreateRootJson(interactionElement: iel, conversationId: _conversationId,
                            messageId: maintenanceData.MessageId, receiver: sender);
                        try
                        {
                            _ = Task.Run(() =>
                            {
                                MqttPublisherAndReceiver.MqttPublishAsync(MqttPublisherAndReceiver.brockerAddress,
                                    MqttPublisherAndReceiver.brockerPort, "aas-notification", realRoot);
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

            string GetMessageType(RootObject inComingData)
            {
                return inComingData.Frame.Type;
            }
        }
    }
}