using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ComplexAssetAdministrationShellScenario.Serializers
{
    public class UseCase2response
    {
        [JsonProperty("ConversationID")]
        public string ConversationID { get; set; }

        [JsonProperty("MessagID")]
        public string MessagID { get; set; }

        [JsonProperty("DelayPerOrder")]
        public int DelayPerOrder { get; set; }

        [JsonProperty("Month 1")]
        public string Month1 { get; set; }

        [JsonProperty("Turnover 1")]
        public double Turnover1 { get; set; }

        [JsonProperty("Month 2")]
        public string Month2 { get; set; }

        [JsonProperty("Turnover 2")]
        public double Turnover2 { get; set; }

        [JsonProperty("Month 3")]
        public string Month3 { get; set; }

        [JsonProperty("Turnover 3")]
        public double Turnover3 { get; set; }
    }

    public class Usecasetwoie
    {
       
        [JsonProperty("DelayPerOrder")]
        public int DelayPerOrder { get; set; }

        [JsonProperty("Month 1")]
        public string Month1 { get; set; }

        [JsonProperty("Turnover 1")]
        public double Turnover1 { get; set; }

        [JsonProperty("Month 2")]
        public string Month2 { get; set; }

        [JsonProperty("Turnover 2")]
        public double Turnover2 { get; set; }

        [JsonProperty("Month 3")]
        public string Month3 { get; set; }

        [JsonProperty("Turnover 3")]
        public double Turnover3 { get; set; }
        }
    }

