using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ComplexAssetAdministrationShellScenario
{
    public class RootObjectBuilder
    {
        public static string CreateRootJson(
            string modelName = "SubmodelElementCollection",
            string MaintenanceElement = "DMU80eVo1",
            int MaintenanceThreshold = 250,
            string MaintenanceCompany = "Lauscher",
            string MaintenanceCompanyLocation = "Aachen",
            bool ordered = false,
            bool allowDuplicates = false,
            string kind = "Instance",
            string conversation_id = "Maintenance_1::1",
            int message_id = 1
        )
        {
            // Build the JSON object using the provided arguments
            var rootObject = new
            {
                InteractionElements = new List<object>
                {
                    new
                    {
                        ModelType = new { Name = modelName },
                        DataSpecification = new List<object>(),
                        EmbeddedDataSpecifications = new List<object>(),
                        Kind = kind,
                        Value = new Dictionary<string, object>
                        {
                            {
                                "MaintenanceElement", new
                                {
                                    ModelType = new { Name = "Property" },
                                    DataSpecification = new List<object>(),
                                    EmbeddedDataSpecifications = new List<object>(),
                                    Kind = "Instance",
                                    Value = MaintenanceElement,
                                    ValueType = "string",
                                    IdShort = "MaintenanceElement",
                                    Qualifiers = new List<object>(),
                                    SemanticId = new { Keys = new List<object>() }
                                }
                            },
                            {
                                "MaintenanceThreshold", new
                                {
                                    ModelType = new { Name = "Property" },
                                    DataSpecification = new List<object>(),
                                    EmbeddedDataSpecifications = new List<object>(),
                                    Kind = "Instance",
                                    Value = MaintenanceThreshold,
                                    ValueType = "int",
                                    IdShort = "MaintenanceThreshold",
                                    Qualifiers = new List<object>(),
                                    SemanticId = new { Keys = new List<object>() }
                                }
                            },
                            {
                                "MaintenanceCompany", new
                                {
                                    ModelType = new { Name = "Property" },
                                    DataSpecification = new List<object>(),
                                    EmbeddedDataSpecifications = new List<object>(),
                                    Kind = "Instance",
                                    Value = MaintenanceCompany,
                                    ValueType = "string",
                                    IdShort = "MaintenanceCompany",
                                    Qualifiers = new List<object>(),
                                    SemanticId = new { Keys = new List<object>() }
                                }
                            },
                            {
                                "MaintenanceCompanyLocation", new
                                {
                                    ModelType = new { Name = "Property" },
                                    DataSpecification = new List<object>(),
                                    EmbeddedDataSpecifications = new List<object>(),
                                    Kind = "Instance",
                                    Value = MaintenanceCompanyLocation,
                                    ValueType = "string",
                                    IdShort = "MaintenanceCompanyLocation",
                                    Qualifiers = new List<object>(),
                                    SemanticId = new { Keys = new List<object>() }
                                }
                            }
                        },
                        Ordered = ordered,
                        AllowDuplicates = allowDuplicates,
                        IdShort = "MaintenanceOrderDescription",
                        Qualifiers = new List<object>(),
                        SemanticId = new { Keys = new List<object>() }
                    }
                },
                Frame = new
                {
                    Type = "NOTIFY_INIT",
                    Sender = new
                    {
                        Identification = new { Id = "BASYX_MACHINE_AAS_POC", IdType = "CUSTOM" },
                        Role = new { Name = "InformationSender" }
                    },
                    Receiver = new
                    {
                        Identification = new { Id = "MES_AAS", IdType = "CUSTOM" },
                        Role = new { Name = "InformationReceiver" }
                    },
                    ConversationId = conversation_id,
                    MessageId = message_id.ToString(),
                    InReplyTo = (object)null,
                    ReplyBy = (object)null,
                    SemanticProtocol = new
                    {
                        Keys = new List<object>
                        {
                            new { Type = "GlobalReference", IdType = "CUSTOM", Value = "Maintenance" }
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