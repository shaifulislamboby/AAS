using System.Collections.Generic;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;

namespace ComplexAssetAdministrationShellScenario.Serializers
{
    public class I40Message<T>
    {
        
        public List<T> interactionElements { get; set; }
        public Frame frame { get; set; }
        

        public void SetInteractionElement(List<T> interactionElements)
        {
            this.interactionElements = interactionElements;
        }

        public List<T> GetInteracrionElement()
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