using Newtonsoft.Json;
using System;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace HelloAssetAdministrationShell.MqttConnection
{
    public class DatafromMqtt
    {
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Speed { get; set; }
        public string TimeStamp { get;set; }
        public string MachineStatus { get; set; }   

        public void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received 
            // Console.WriteLine("Received: " + System.Text.Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);

            var message = System.Text.Encoding.UTF8.GetString(e.Message);
            Console.WriteLine(message.GetType());

            var jsonObject = JsonConvert.DeserializeObject<dynamic>(message);
            Console.WriteLine(jsonObject);

            //  Console.WriteLine("Temperature: " + jsonObject["Temperature"]);
            //   Console.WriteLine("Speed: " + jsonObject["Speed"]);


             this.Temperature = jsonObject["Temperature"].ToString();
             this.Humidity = jsonObject["Humidity"].ToString();
            this.Speed = jsonObject["Speed"];
             this.TimeStamp = jsonObject["TimeStamp"];
            this.MachineStatus = jsonObject["MachineStatus"];
           

        }

        public string getTemperature()
        {
            return Temperature;
        }

        public string getHumidity()
        {
            return Humidity;

        }

        public string gerSpeed()
        {
            return Speed;
        }

        public string getTimeStamp() { return TimeStamp; }

    }
    

}
