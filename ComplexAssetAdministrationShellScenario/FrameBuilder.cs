using System.Collections.Generic;

namespace ComplexAssetAdministrationShellScenario
{

    public class FrameBuilder
    {
        public static Frame CreateFrame(string receiver,
            
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
            var frame = new Frame
            {
                type = messageType,
                sender = new Sender
                {
                    identification = new Identification
                    {


                        id ="MES_AAS",
                        idType = "CUSTOM",


                    },
                    role = new Role
                    {
                        name = "InformationSender"
                    }

                },
                receiver = new Receiver
                {
                    identification = new Identification
                    {

                        id = receiver,
                        idType = "CUSTOM"
                    },
                    role = new Role
                    {
                        name = "informationReceiver"
                    }
                },
                conversationId = conversationId,
                messageId = messageId,
                replyBy = null,
                semanticProtocol = new SemanticProtocol
                {
                   keys = new List<SemanticProtocolKey>
                    
                    {

                        new SemanticProtocolKey
                        {
                            type = "GlobalReference",
                            idType ="CUSTOM",
                            value ="Maintenance"
                        }

                    }
                }

            };
            return frame;
        }
        }
    }