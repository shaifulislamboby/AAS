using System.Collections.Generic;

namespace ComplexAssetAdministrationShellScenario
{
    public class ModelType
    {
        public string Name { get; set; }
    }

    public class Property
    {
        public string IdShort { get; set; }
        public ModelType ModelType { get; set; }
        public object Value { get; set; }
        public string ValueType { get; set; }
    }

    public class InteractionElement
    {
        public string IdShort { get; set; }
        public ModelType ModelType { get; set; }
        public List<Property> Value { get; set; }
    }

    public class Sender
    {
        public Identification Identification { get; set; }
        public Role Role { get; set; }
    }

    public class Receiver
    {
        public Identification Identification { get; set; }
        public Role Role { get; set; }
    }

    public class SemanticProtocolKey
    {
        public string Type { get; set; }
        public string IdType { get; set; }
        public string Value { get; set; }
    }

    public class Frame
    {
        public string Type { get; set; }
        public Sender Sender { get; set; }
        public Receiver Receiver { get; set; }
        public string ConversationId { get; set; }
        public int MessageId { get; set; }
        public object InReplyTo { get; set; }
        public object ReplyBy { get; set; }
        public SemanticProtocol SemanticProtocol { get; set; }
    }

    public class RootObject
    {
        public List<InteractionElement> InteractionElements { get; set; }
        public Frame Frame { get; set; }
    }

    public class SemanticProtocol
    {
        public List<SemanticProtocolKey> Keys { get; set; }
    }


    public class Identification
    {
        public string Id { get; set; }
        public string IdType { get; set; }
    }

    public class Role
    {
        public string Name { get; set; }
    }
}