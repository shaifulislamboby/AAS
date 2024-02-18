using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
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
        public static string AASOrderAcceptedNotificationAcknowledged = "AASOrderAcceptedNotificationAcknowledged";
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
        
        private static async Task RetryPolicy(string retryMessage, string conversationId, DataStorage dataStorage)
        {
            Console.WriteLine("Application is starting retry policy for notify ACCEPTED");

            for (int retry = 0; retry < 5; retry++)
            {
                await Task.Delay(5000);
                var rDictionary = dataStorage.dataDictionary[conversationId];


                if (rDictionary["OrderStatus"] == OrderStatus.OrderAccepted)
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

                else if (rDictionary["OrderStatus"] == OrderStatus.AASOrderAcceptedNotificationAcknowledged)
                    
                {
                    Console.WriteLine("We are stopping the retry policy for order accepted, conversation id: " + conversationId);
                    break;
                }
                
            }
        }

        public static async Task call_maintence_endpoint_usecase2(string jsonData)
        {
            _data = JsonConvert.DeserializeObject(jsonData);
            var ie = _data.interactionElements;
            Console.WriteLine(_data);
            List<Interrup> interrups = JsonConvert.DeserializeObject<List<Interrup>>(ie.ToString());
            _conversationId = _data.frame.conversationId;
            sender = _data.frame.sender.identification.id;
            Usecase2data da = new Usecase2data()
            {
                ConversationID = _conversationId,
                Interruptions = interrups,
                MessagID = 1.ToString()
            };
            var requestdata = JsonConvert.SerializeObject(da).ToString();
           
            string url = Program.Configuration["MES_APPLICATION_CONFIG:MES_ENDPOINT_USE_CASE2"];
            try
            {
                HttpClient client = new HttpClient();
                StringContent content = new StringContent(requestdata, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<UseCase2response>(responseContent);
                    List<Usecasetwoie> ieusecsetwo = new List<Usecasetwoie>();
                    Usecasetwoie ietwo = new Usecasetwoie()
                    {
                        DelayPerOrder = data.DelayPerOrder,
                        Month1 = data.Month1,
                        Turnover1 = data.Turnover1,
                        Month2 = data.Month2,
                        Turnover2 = data.Turnover2,
                        Month3 = data.Month3,
                        Turnover3 = data.Turnover3
                    };
                    ieusecsetwo.Add(ietwo);
                    I40Message<Usecasetwoie> message = new I40Message<Usecasetwoie>();
                    message.interactionElements = ieusecsetwo;
                  var frame =  FrameBuilder.CreateFrame(sender.ToString(), _conversationId, 10, "ACKNOWLEDGE");
                  message.frame = frame;
                  var outBoundMessageString = JsonConvert.SerializeObject(message);
                  Console.WriteLine(message);

                  
                  try
                  {
                      _ = Task.Run(() =>
                      {
                          MqttPublisherAndReceiver.MqttPublishAsync(MqttPublisherAndReceiver.brockerAddress,
                              MqttPublisherAndReceiver.brockerPort,Program.Configuration["MES_APPLICATION_CONFIG:PUBLICATION_TOPIC_USE_CASE2"] ,outBoundMessageString);
                          return Task.CompletedTask;
                      });

                  }
                  catch (Exception e)
                  {
                      Console.WriteLine(e);
                      throw;
                  }


                }
                else
                {
                        Console.WriteLine("Message cannot be sent http request is not successful ");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

           
           
        }
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
                    if (storage.dataDictionary.ContainsKey(_conversationId))
                    {
                        var data = storage.dataDictionary[_conversationId];
                        bool value = data.ContainsKey("OrderStatus");
                        if (value == true && data["OrderStatus"] == OrderStatus.AASOrderCompletionNotification)
                        {
                            data["OrderStatus"] = OrderStatus.OrderCompleted;
                        }
                        else if(value ==true && data["OrderStatus"] == OrderStatus.OrderAccepted)
                        {
                            data["OrderStatus"] = OrderStatus.AASOrderAcceptedNotificationAcknowledged;
                        }
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
                            //Creating new I4.0 message
                        //    I40Message outBoundMessage = new I40Message();
                        I40Message<SubmodelElementCollection> outBoundMessage =
                            new I40Message<SubmodelElementCollection>();
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
                          
                            await RetryPolicy(outBoundMessageString,_conversationId,storage);


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