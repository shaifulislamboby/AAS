using System.Collections.Generic;
using System.Text.Json;

namespace ComplexAssetAdministrationShellScenario
{
    public class RootObjectBuilder
    {
        public static string CreateRootJson(string receiver, List<string> interactionElement,
            //string modelName = "SubmodelElementCollection",
            //string maintenanceElement = "DMU80eVo1",
            //int maintenanceThreshold = 250,
            //string maintenanceCompany = "Lauscher",
            //string maintenanceCompanyLocation = "Aachen",
            //bool ordered = false,
            //bool allowDuplicates = false,
            //string kind = "Instance",
            string conversationId = "Maintenance_1::1", int messageId = 1, string messageType = "notify_accepted")
        {
            // Build the JSON object using the provided arguments
            var rootObject = new
            {
                interactionElements = interactionElement,
                frame = new
                {
                    type = messageType,
                    sender =
                        new
                        {
                            identification = new { id = "MES_AAS", idType = "CUSTOM" },
                            role = new { name = "InformationSender" }
                        },
                    receiver =
                        new
                        {
                            identification = new { id = receiver, idType = "CUSTOM" },
                            role = new { name = "InformationReceiver" }
                        },
                    conversationId = conversationId,
                    messageId = messageId.ToString(),
                    inReplyTo = (object)null,
                    replyBy = (object)null,
                    semanticProtocol = new
                    {
                        keys = new List<object>
                        {
                            new { type = "GlobalReference", idType = "CUSTOM", value = "Maintenance" }
                        }
                    }
                }
            };

            // Convert the object to JSON
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(rootObject, options);
        }
    }
}