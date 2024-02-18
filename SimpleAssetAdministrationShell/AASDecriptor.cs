using System.Collections.Generic;
using BaSyx.Models.Connectivity;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.Common;

namespace SimpleAssetAdministrationShell
{
    public class AASDecriptor:IAssetAdministrationShellDescriptor
    {
        public IEnumerable<IEndpoint> Endpoints { get; }
        public string IdShort { get; }
        public string Category { get; }
        public LangStringSet Description { get; }
        public IReferable Parent { get; set; }
        public Identifier Identification { get; }
        public AdministrativeInformation Administration { get; }
        public void AddEndpoints(IEnumerable<IEndpoint> endpoints)
        {
           
        }

        public void SetEndpoints(IEnumerable<IEndpoint> endpoints)
        {
            
        }

        public ModelType ModelType { get; }
        public IAsset Asset { get; set; }
        public IElementContainer<ISubmodelDescriptor> SubmodelDescriptors { get; set; }
    }
}