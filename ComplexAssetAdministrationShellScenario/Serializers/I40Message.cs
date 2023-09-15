using System.Collections.Generic;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;

namespace ComplexAssetAdministrationShellScenario.Serializers
{
    public class I40Message
    {
       

        public List<SubmodelElementCollection> interactionElements { get; set; }
        public Frame frame { get; set; }
        

        public void SetInteractionElement(List<SubmodelElementCollection> interactionElements)
        {
            this.interactionElements = interactionElements;
        }

        public List<SubmodelElementCollection> GetInteracrionElement()
        {
            return interactionElements;
        }

        public void Setframe(Frame frame)
        {
            this.frame = frame;
        }

        public Frame GetFrame()
        {
            return this.frame;
        }

    }
}