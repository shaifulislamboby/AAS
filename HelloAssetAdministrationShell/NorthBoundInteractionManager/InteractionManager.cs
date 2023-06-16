using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using I40Extension.UtilsMqtt;
using I40Extension.Message;



namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public class InteractionManager
    {
        private readonly AssetAdministrationShellHttpClient client;

        


        public async task<SubmodelElementCollection> GetInteractionElement(Uri uri, string SubmodelIdShort, string SubmodelElementidShort)
        {
            try
            {
                AssetAdministrationShellHttpClient client = new AssetAdmistationShellClient(new Uri("http://localhost:5180"));

                interactionElements e = new interactionElements();

              await e = client.RetrieveSubmodelElement(SubmodelIdShort, SubmodelElementidShort);
                return e;
            }
            catch
            {

                ILogger.log("Cannot connect to aas");
            }
           
        }

    }
}
