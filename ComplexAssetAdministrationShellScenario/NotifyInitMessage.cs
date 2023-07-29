namespace ComplexAssetAdministrationShellScenario
{
    using System.Collections.Generic;

    public abstract class ModelType
    {
        public string Name { get; set; }
    }

    public abstract class SemanticId
    {
        public List<object> Keys { get; set; }
    }

    public abstract class MaintenanceElement
    {
        public ModelType ModelType { get; set; }
        public List<object> DataSpecification { get; set; }
        public List<object> EmbeddedDataSpecifications { get; set; }
        public string Kind { get; set; }
        public string Value { get; set; }
        public object ValueId { get; set; }
        public string ValueType { get; set; }
        public string IdShort { get; set; }
        public List<object> Qualifiers { get; set; }
        public SemanticId SemanticId { get; set; }
    }

    public abstract class InteractionElement
    {
        public ModelType ModelType { get; set; }
        public List<object> DataSpecification { get; set; }
        public List<object> EmbeddedDataSpecifications { get; set; }
        public string Kind { get; set; }
        public Dictionary<string, MaintenanceElement> Value { get; set; }
        public bool Ordered { get; set; }
        public bool AllowDuplicates { get; set; }
        public string IdShort { get; set; }
        public List<object> Qualifiers { get; set; }
        public SemanticId SemanticId { get; set; }
    }

    public class Identification
    {
        public string Id { get; set; }
        public string IdType { get; set; }
    }

    public abstract class Role
    {
        public string Name { get; set; }
    }

    public abstract class Sender
    {
        public Identification Identification { get; set; }
        public Role Role { get; set; }
    }

    public class Receiver
    {
        public Identification Identification { get; set; }
        public Role Role { get; set; }
    }

    public abstract class SemanticProtocol
    {
        public List<object> Keys { get; set; }
    }

    public abstract class Frame
    {
        public string Type { get; set; }
        public Sender Sender { get; set; }
        public Receiver Receiver { get; set; }
        public string ConversationId { get; set; }
        public string MessageId { get; set; }
        public object InReplyTo { get; set; }
        public object ReplyBy { get; set; }
        public SemanticProtocol SemanticProtocol { get; set; }
    }

    public class RootObject
    {
        public List<InteractionElement> InteractionElements { get; set; }
        public Frame Frame { get; set; }
    }
}