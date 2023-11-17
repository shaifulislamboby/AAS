namespace ComplexAssetAdministrationShellScenario.MesSettings
{
    public class MesSettings
    {
   /* "BROKER_ADDRESS":"test.mosquitto.org",
    "BROKER_PORT": 1883,
    "SUBSCRIPTION_TOPIC": "BasyxMesAASOrderHandling", 
    "PUBLICATION_TOPIC": "aas-notification",
    "MES_ENDPOINT" : "http://localhost:5180/maintenance_request/"*/
        public string  BrokerAddress { get; set; }
        public int BrokerPort { get; set; }
        public string SubscriptionTopic { get; set; }
        public string MesHttpEndpoint { get; set; }
    }
}