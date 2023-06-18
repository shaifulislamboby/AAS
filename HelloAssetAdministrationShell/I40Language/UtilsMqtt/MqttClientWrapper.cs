using MQTTnet;
using MQTTnet.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using MQTTnet.Protocol;

namespace I40Extension.UtilsMqtt
{
    public class MqttClientWrapper : IDisposable
    {

        private readonly IMqttClient _mqttClient;

        public event EventHandler<MqttApplicationMessageReceivedEventArgs> MessageReceived;

 
        public MqttClientWrapper(string brokerIpAddress, int brokerPort)
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerIpAddress, brokerPort)
            .WithCleanSession()
            .Build();

           
            _mqttClient = new MqttFactory().CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessages;

            _mqttClient.ConnectAsync(options);

        }

        
        private async Task HandleReceivedMessages(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            string topic = eventArgs.ApplicationMessage.Topic;
            string message = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);

            MessageReceived?.Invoke(this, eventArgs);

            Console.WriteLine($"Received message on topic: {topic}");
            Console.WriteLine($"Message: {message}");

            // Perform any additional processing or logic here

            await Task.CompletedTask;
        }


        public async Task PublishAsync<I40Message>(string topic, I40Message payload)
        {
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(jsonPayload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .Build();

            await _mqttClient.PublishAsync(message);
        }

        public void Subscribe(string topic)
        {
            _mqttClient.SubscribeAsync(topic).Wait();
        }
        
        public void Dispose()
        {
            _mqttClient.DisconnectAsync().Wait();
            _mqttClient.Dispose();
        }
    }
}
