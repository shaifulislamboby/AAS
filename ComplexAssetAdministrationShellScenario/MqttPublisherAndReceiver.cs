 using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ComplexAssetAdministrationShellScenario
{
    public static class MqttPublisherAndReceiver
    {
        private static MqttClient mqttClient;
        private static List<string> receivedMessages = new List<string>();
        private static DataStorage mainDataStorage;
        private static string publishingData;
        public static string brockerAddress;
        public static int brockerPort;
        public static List<string> Substcriptiontopics = new List<string>();
        private static void InitializeMqttClient(string brokerAddress, int brokerPort)
        {
            mqttClient = new MqttClient(brokerAddress, brokerPort, false, MqttSslProtocols.None, null, null);
            mqttClient.MqttMsgPublishReceived += MqttMsgPublishReceived;
        }

        private static async void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string topic = e.Topic.ToString();
            string message = Encoding.UTF8.GetString(e.Message);
            receivedMessages.Add(message);
            Console.WriteLine("Received message: " + message);
            if (topic == Substcriptiontopics[0])
            {
                await MaintenanceProvoking.call_maintenance_endpoint(message, mainDataStorage, publishingData);
            }
            else
            {
                await MaintenanceProvoking.call_maintence_endpoint_usecase2(message);
            }
        }

        public static void MqttPublishAsync(string brokerAddress, int brokerPort, string topic, string message)
        {
            if (mqttClient == null)
            {
                InitializeMqttClient(brokerAddress, brokerPort);
            }

            byte[] payload = Encoding.UTF8.GetBytes(message);
            mqttClient.Publish(topic, payload, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        public static async Task MqttSubscribeAsync(string brokerAddress, int brokerPort, List<string> topics, DataStorage ma,
            string pubdata)
        {
            mainDataStorage = ma;
            publishingData = pubdata;
            brockerAddress = brokerAddress;
            brockerPort = brokerPort;
            Substcriptiontopics = topics;
            if (mqttClient == null)
            {
                InitializeMqttClient(brokerAddress, brokerPort);
            }

            string clientId = Guid.NewGuid().ToString();
            mqttClient.Connect(clientId);
            string[] topicsArray = topics.ToArray();
    
            byte[] qosLevels = new byte[topicsArray.Length];
            for (int i = 0; i < qosLevels.Length; i++)
            {
                qosLevels[i] = MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE;
            }
    
            mqttClient.Subscribe(topicsArray, qosLevels);

            // The subscriber should keep listening until you explicitly stop it.
        }

        public static List<string> GetReceivedMessages()
        {
            return receivedMessages;
        }
    }
}