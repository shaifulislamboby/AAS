using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ComplexAssetAdministrationShellScenario
{
    public class MaintenanceProvoking

    { 
        public static async Task  call_maintenance_endpoint(string jsonData, DataStorage storage, string realRoot)
    {
        try
        {
            dynamic data = JsonConvert.DeserializeObject(jsonData);
            RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(jsonData);
            Dictionary<string, object> dataDictionary = new Dictionary<string, object>();


            string conversationId = data.frame.conversationId;
            string messageId = data.frame.messageId;
            string machineName = data.frame.sender.identification.id;
            string maintenanceThreshold = data.interactionElements[0].value.MaintenanceThreshold.value;
            
            dataDictionary.Add("data", rootObject);
            dataDictionary.Add("messageId", messageId);
            dataDictionary.Add("maintenanceThreshold", maintenanceThreshold);
            dataDictionary.Add("machineName", machineName);
            storage.SaveData(conversationId, dataDictionary);
            
            
            foreach (var kvp in storage.dataDictionary)
            {
                Console.WriteLine($"Name: {kvp.Key}, Age: {kvp.Value}");
                foreach (var kv in kvp.Value)
                {
                    Console.WriteLine($"Name: {kv.Key}, Age: {kv.Value}");   
                }
            }
            
            string url = "http://127.0.0.1:8000/maintenance-request/"; // Replace with the actual API endpoint URL

            // Create a JSON string containing the arguments as data
            string Data = $"{{ \"conversationId\": \"{conversationId}\", \"MessageId\": \"{messageId}\", \"MachineName\": \"{machineName}\", \"MaintenanceThreshold\": {maintenanceThreshold} }}";

            // Create a new HttpClient instance
            using (HttpClient client = new HttpClient())
            {
                // Set any required headers, for example, Content-Type
                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

                // Make the POST request
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(Data, Encoding.UTF8, "application/json"));
                Console.WriteLine(response);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("POST request successful!");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    //JObject jsonResponse = JObject.Parse(responseContent);
                    JObject jsonResponse = JObject.Parse(responseContent);
                    JObject successObject = jsonResponse["success"] as JObject;

                    // Deserialize the successObject into a MaintenanceSerializer instance
                    MaintenanceSerializer maintenanceData = successObject.ToObject<MaintenanceSerializer>();

                    // Get the inner dictionary from the "success" key
                    // Deserialize the response content to your desired object
                    storage.ModifyInnerValue(maintenanceData.ConversationId, "messageId", maintenanceData.MessageId);
                    
                    // Now you can work with the parsed object
                    Console.WriteLine(maintenanceData.MessageId);
                    realRoot = RootObjectBuilder.CreateRootJson(message_id: maintenanceData.MessageId);

                    try
                    {
                        _ = Task.Run(() =>
                        {
                            MqttPublisherAndReceiver.MqttPublishAsync(MqttPublisherAndReceiver.brockerAddress, MqttPublisherAndReceiver.brockerPort, "aas-notification", realRoot);
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during the POST request: {ex.Message}");
        }
    }
}
}