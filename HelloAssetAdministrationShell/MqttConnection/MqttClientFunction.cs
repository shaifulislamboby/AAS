using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

using CsvHelper;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.Net.Http;
using System.Text.Json;
namespace HelloAssetAdministrationShell.MqttConnection
{
    public class MqttClientFunction
    {
        private MqttClient client;
        private HttpClient httpClient;
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Speed { get; set; }
        public string TimeStamp { get; set; }
        public string MachineStatus { get; set; }

        private StringBuilder csv;


        public MqttClientFunction()
        {
            // create client instance 
            client = new uPLibrary.Networking.M2Mqtt.MqttClient("test.mosquitto.org");

            // register to message received 
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);

            // subscribe to the topic "/home/temperature" with QoS 2 
            client.Subscribe(new string[] { "MacnineData/ID-0000" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            
        }

        static async void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received 
            // Console.WriteLine("Received: " + System.Text.Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);

            var message = System.Text.Encoding.UTF8.GetString(e.Message);
            Console.WriteLine(message.GetType());

            var jsonObject = JsonConvert.DeserializeObject<dynamic>(message);
            Console.WriteLine(jsonObject);
            Console.WriteLine("Temperature: " + jsonObject["Temperature"]);
            Console.WriteLine("Speed: " + jsonObject["Speed"]);
            Console.WriteLine(jsonObject["Timestamp"]);
            Console.WriteLine(jsonObject["MachineStatus"]);

            var Temperature = jsonObject["Temperature"].ToString();
         // var temp = jsonObject["Temperature"].ToSting();
            var Humidity = jsonObject["Humidity"].ToString();
            var Speed = jsonObject["Speed"];
            var TimeStamp = jsonObject["Timestamp"];
            var MachineStatus = jsonObject["MachineStatus"];
            Console.WriteLine("current Temperture is " + Temperature);
            Console.WriteLine("current Temperture is " + MachineStatus);

            
            
                var csv = new StringBuilder();
                var newLine = string.Format("Temperature : {0}, Humidity : {1}, Speed : {2}, TimeStamp : {3}, MachineStatus : {4}", Temperature, Humidity, Speed, TimeStamp, MachineStatus);
                csv.AppendLine(newLine);
                File.WriteAllText("data.csv", csv.ToString());
                string mes = JsonConvert.SerializeObject(Temperature);
                HttpClient client = new HttpClient();

            try
            {
                var res = await client.PutAsync("http://172.28.208.1:5180/aas/submodels/HelloSubmodel/submodel/submodelElements/Temperature/value", new StringContent(mes,
                    Encoding.UTF8, "application/json"));
                Console.WriteLine("Data send to submodel Element");
                if (MachineStatus == "on")
                {


                    var counterVal = await client.GetAsync("http://172.28.208.1:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance250H/OperationCounter250H/value");

                    if (counterVal.IsSuccessStatusCode)
                    {
                        var CurrentCounterValue = await counterVal.Content.ReadAsStringAsync();
                        // int currentdata = Convert.ToInt32(CurrentCounterValue);
                        Console.WriteLine($"The counter value is : {0}", CurrentCounterValue);
                        int cv = Convert.ToInt32(CurrentCounterValue);
                        Console.WriteLine(cv);
                        int increamnetCounterVal = cv+1;
                        Console.WriteLine(increamnetCounterVal);
                       string counterupdate = JsonConvert.SerializeObject(increamnetCounterVal);
                        var update = await client.PutAsync("http://172.28.208.1:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance250H/OperationCounter250H/value", new StringContent(counterupdate,
                               Encoding.UTF8, "application/json"));
                        Console.WriteLine("CountervalueUpdated");


                        

                        /* int counter = Convert.ToInt32(counterVal.Content.ToString());
                         Console.WriteLine($"counter value in in int data type {0}", counter);
                         var updetedcountervalue = counter + 1;
                         Console.WriteLine(updetedcountervalue);
                         //   var res1 = await client.PutAsync("http://172.28.208.1:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance250H/OperationCounter250H/value",new StringContent)
                  */
                    }



                    else { Console.WriteLine(MachineStatus); }
                }
            }
            catch
            {
                Console.WriteLine("Unable to connect");

            }
         
        }

     
        

        //after your loop




        public string getMachineDataTimeStamp()
        {

            return TimeStamp;

        }
        public string getMachineDataSpeed() { return Speed; }

        public string getMachineDataHumidity() { return Humidity; }
        public string getMachineDataTemperature() { return Speed; }

    }
}

