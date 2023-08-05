
    using System;
    using System.Text.Json.Serialization;

    namespace ComplexAssetAdministrationShellScenario
    {
        public class MaintenanceSerializer
        {
            [JsonPropertyName("conversationId")]
            public string ConversationId { get; set; }

            [JsonPropertyName("MessageId")]
            public int MessageId { get; set; }

            [JsonPropertyName("MachineName")]
            public string MachineName { get; set; }

            [JsonPropertyName("MaintenanceThreshold")]
            public int MaintenanceThreshold { get; set; }

            [JsonPropertyName("PlannedMaintenanceStart")]
            public string PlannedMaintenanceStart { get; set; } = DateTime.Now.ToString();

            [JsonPropertyName("PlannedMaintenanceEnd")]
            public string PlannedMaintenanceEnd { get; set; } = DateTime.Now.ToString();
        }
    }