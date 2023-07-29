using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ComplexAssetAdministrationShellScenario
{
    public static class MqttPublisherAndReceiver
    {
        private static MqttClient mqttClient;
        private static List<string> receivedMessages = new List<string>();

        private static void InitializeMqttClient(string brokerAddress, int brokerPort)
        {
            mqttClient = new MqttClient(brokerAddress, brokerPort, false, MqttSslProtocols.None, null, null);
            mqttClient.MqttMsgPublishReceived += MqttMsgPublishReceived;
        }

        private static void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);
            receivedMessages.Add(message);
            Console.WriteLine("Received message: " + message);
        }

        public static async Task MqttPublishAsync(string brokerAddress, int brokerPort, string topic, string message)
        {
            if (mqttClient == null)
            {
                InitializeMqttClient(brokerAddress, brokerPort);
            }

            string clientId = Guid.NewGuid().ToString();
            mqttClient.Connect(clientId);

            byte[] payload = Encoding.UTF8.GetBytes(message);
            mqttClient.Publish(topic, payload, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);

            mqttClient.Disconnect();
        }

        public static async Task MqttSubscribeAsync(string brokerAddress, int brokerPort, string topic)
        {
            if (mqttClient == null)
            {
                InitializeMqttClient(brokerAddress, brokerPort);
            }

            string clientId = Guid.NewGuid().ToString();
            mqttClient.Connect(clientId);

            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
            mqttClient.Subscribe(new string[] { topic }, qosLevels);

            // The subscriber should keep listening until you explicitly stop it.
        }

        public static List<string> GetReceivedMessages()
        {
            return receivedMessages;
        }
    }
}
