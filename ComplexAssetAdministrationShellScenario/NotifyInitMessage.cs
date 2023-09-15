using System.Collections.Generic;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace ComplexAssetAdministrationShellScenario
{
    public class ModelType
    {
        public string name { get; set; }
    }

    public class Property
    {
        public string idShort { get; set; }
        public ModelType modelType { get; set; }
        public object value { get; set; }
        public string valueType { get; set; }
    }

    public class InteractionElement
    {
        public string idShort { get; set; }
        public ModelType modelType { get; set; }
        public List<Property> value { get; set; }
    }

    public class Sender
    {
        public Identification identification { get; set; }
        public Role role { get; set; }
    }

    public class Receiver
    {
        public Identification identification { get; set; }
        public Role role { get; set; }
    }

    public class SemanticProtocolKey
    {
        public string type { get; set; }
        public string idType { get; set; }
        public string value { get; set; }
    }

    public class Frame
    {
        public string type { get; set; }
        public Sender sender { get; set; }
        public Receiver receiver { get; set; }
        public string conversationId { get; set; }
        public int messageId { get; set; }
        public object inReplyTo { get; set; }
        public object replyBy { get; set; }
        public SemanticProtocol semanticProtocol { get; set; }
    }

    public class RootObject
    {
        public List<ISubmodelElementCollection> interactionElements { get; set; }
        public Frame frame { get; set; }
    }

    public class SemanticProtocol
    {
        public List<SemanticProtocolKey> keys { get; set; }
    }


    public class Identification
    {
        public string id { get; set; }
        public string idType { get; set; }
    }

    public class Role
    {
        public string name { get; set; }
    }
}