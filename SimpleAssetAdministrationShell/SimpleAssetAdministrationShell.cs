/*****************************************************************************/

using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.Attributes;
using BaSyx.Utils.DependencyInjection;
using Microsoft.AspNetCore.Connections;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleAssetAdministrationShell
{
    public static class SimpleAssetAdministrationShell
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private static JsonSerializerSettings JsonSerializerSettings { set; get; }
        private static string modifiedJsonResponse { get; set; }
        private static AssetAdministrationShell shell { get; set; }
       

        public static AssetAdministrationShell GetAssetAdministrationShell()
        {
         /*   var aasdb = GetAasFromDB();

           // if (aasdb! == null)
            {
                return aasdb;
            }
         //   else
            {*/
                var aas = new AssetAdministrationShell("MESAAS", new BaSyxShellIdentifier("MESAAS", "1.0.0"))
                {
                    Description =
                        new LangStringSet()
                        {
                            new LangString("de-DE", "Einfache VWS"), new LangString("en-US", "Simple AAS")
                        },
                    Administration = new AdministrativeInformation() { Version = "1.0", Revision = "120" },
                    Asset = new Asset("SimpleAsset", new BaSyxAssetIdentifier("SimpleAsset", "1.0.0"))
                    {
                        Kind = AssetKind.Instance,
                        Description = new LangStringSet()
                        {
                            new LangString("de-DE", "Einfaches Asset"), new LangString("en-US", "Simple Asset")
                        }
                    }
                };
                var testSubmodel = GetTestSubmodel();
                var MaintenanceOrderHandlingSubmodel = GetMesOrderHandlingSubmodel();
                aas.Submodels.Add(testSubmodel);
                aas.Submodels.Add(MaintenanceOrderHandlingSubmodel);
                return aas;
            }
        

        static void ModifyJsonProperties(JObject jsonObject)
{
    // Properties to modify
    string[] propertiesToModify = { "identification", "administration", "description", "endpoints", "modelType", "asset", "submodels" };

    // Loop through properties and modify
    foreach (var property in propertiesToModify)
    {
        if (jsonObject.ContainsKey(property))
        {
            // Convert the property value to a JObject
            JToken propertyValue = jsonObject[property];

            if (propertyValue.Type == JTokenType.String)
            {
                // Replace the property with the modified value
                jsonObject[property] = JToken.Parse(propertyValue.ToString());
            }
        }
    }
}
    public static Submodel GetMesOrderHandlingSubmodel()
{
    var maintenanceOrderHandlingSubmodel =
        new
            Submodel("MaintenanceOrderHandlingSubmodel",
                new BaSyxSubmodelIdentifier("MaintenanceOrderHandlingSubmodel", "1.0."))
            {
                SubmodelElements =
                {
                    new SubmodelElementCollection("DMU80eVo1")
                    {
                        Value =
                        {
                            new SubmodelElementCollection("Maintenance_1")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement",
                                                "DMU80eVo1"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany",
                                                "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation",
                                                "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            },
                            new SubmodelElementCollection("Maintenance_2")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80eVo1"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                    //add new Values here 
                                }
                            },
                            new SubmodelElementCollection("Maintenance_3")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80eVo1"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    }, //add rest of the property here   new SubmodelElementCollection("MaintenanceHistory")
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            }
                        }
                    }, // new Submodel Element Collection
                    new SubmodelElementCollection("DMU80")
                    {
                        Value =
                        {
                            new SubmodelElementCollection("Maintenance_1")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement",
                                                "DMU80"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany",
                                                "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation",
                                                "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            },
                            new SubmodelElementCollection("Maintenance_2")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "500HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 500)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80"),
                                            new Property<int>("MaintenanceThreshold", 500),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                    //add new Values here 
                                }
                            },
                            new SubmodelElementCollection("Maintenance_3")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "1000HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 1000)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80"),
                                            new Property<int>("MaintenanceThreshold", 1000),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    }, //add rest of the property here   new SubmodelElementCollection("MaintenanceHistory")
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            }
                        }
                    }, // new Submodel Element Collection
                    new SubmodelElementCollection("DMU80evo2")
                    {
                        Value =
                        {
                            new SubmodelElementCollection("Maintenance_1")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80eVo1"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            },
                            new SubmodelElementCollection("Maintenance_2")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80eVo1"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                    //add new Values here 
                                }
                            },
                            new SubmodelElementCollection("Maintenance_3")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80eVo1"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    }, //add rest of the property here   new SubmodelElementCollection("MaintenanceHistory")
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            }
                        }
                    }, new SubmodelElementCollection("DMU80eVo")
                    {
                        Value =
                        {
                            new SubmodelElementCollection("Maintenance_1")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80eVo"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            },
                            new SubmodelElementCollection("Maintenance_2")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80eVo1"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                    //add new Values here 
                                }
                            },
                            new SubmodelElementCollection("Maintenance_3")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80eVo1"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    }, //add rest of the property here   new SubmodelElementCollection("MaintenanceHistory")
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            }
                        }
                    },
                    new SubmodelElementCollection("DMU80evo3")    {
                        Value =
                        {
                            new SubmodelElementCollection("Maintenance_1")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement",
                                                "DMU80evo3"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany",
                                                "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation",
                                                "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            },
                            new SubmodelElementCollection("Maintenance_2")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80evo3"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                    //add new Values here 
                                }
                            },
                            new SubmodelElementCollection("Maintenance_3")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 1000)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80evo3"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    }, //add rest of the property here   new SubmodelElementCollection("MaintenanceHistory")
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            }
                        }
                    },
                      new SubmodelElementCollection("DMU75")
                    {
                        Value =
                        {
                            new SubmodelElementCollection("Maintenance_1")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement",
                                                "DMU75"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany",
                                                "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation",
                                                "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            },
                            new SubmodelElementCollection("Maintenance_2")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU75"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                    //add new Values here 
                                }
                            },
                            new SubmodelElementCollection("Maintenance_3")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU75"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    }, //add rest of the property here   new SubmodelElementCollection("MaintenanceHistory")
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            }
                        }
                    },new SubmodelElementCollection("DMU80P2")
                    {
                        Value =
                        {
                            new SubmodelElementCollection("Maintenance_1")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement",
                                                "DMU80P2"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany",
                                                "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation",
                                                "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            },
                            new SubmodelElementCollection("Maintenance_2")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80P2"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                    //add new Values here 
                                }
                            },
                            new SubmodelElementCollection("Maintenance_3")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80P2"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    }, //add rest of the property here   new SubmodelElementCollection("MaintenanceHistory")
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            }
                        }
                    },new SubmodelElementCollection("DMU80P3")
                    {
                        Value =
                        {
                            new SubmodelElementCollection("Maintenance_1")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement",
                                                "DMU80P3"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany",
                                                "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation",
                                                "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            },
                            new SubmodelElementCollection("Maintenance_2")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80P3"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                    //add new Values here 
                                }
                            },
                            new SubmodelElementCollection("Maintenance_3")
                            {
                                Value =
                                {
                                    new SubmodelElementCollection("MaintenanceDetails")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceName",
                                                "250HourMaintenance"),
                                            new Property<int>("MaintenanceInterval", 250)
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderDescription")
                                    {
                                        Value =
                                        {
                                            new Property<string>("MaintenanceElement", "DMU80P3"),
                                            new Property<int>("MaintenanceThreshold", 250),
                                            new Property<string>("MaintenanceCompany", "Lauscher"),
                                            new Property<string>("MaintenanceCompanyLocation", "Achen")
                                        }
                                    },
                                    new SubmodelElementCollection("MaintenanceOrderStatus")
                                    {
                                        Value = { new Property<string>("ActualOrderStatus") }
                                    },
                                    new SubmodelElementCollection("MaintenanceRecord")
                                    {
                                        Value =
                                        {
                                            new Property<DateTime>("PlannedMaintenanceStart"),
                                            new Property<DateTime>("PlannedMaintenanceEnd"),
                                            new Property<DateTime>("ActualMaintenanceStart"),
                                            new Property<DateTime>("ActualMaintenanceEnd"),
                                            new Property<double>("MaintenanceCompletionTime"),
                                            new Property<string>("MaintenanceStaff"),
                                            new Property<double>("MaintenanceCosts")
                                        }
                                    }, //add rest of the property here   new SubmodelElementCollection("MaintenanceHistory")
                                    new SubmodelElementCollection("MaintenanceHistory")
                                    {
                                        Value = { new Property<int>("MaintenanceCounter", 0) }
                                    }
                                }
                            }
                        }
                    },
                    
                }
            };
    return maintenanceOrderHandlingSubmodel;
}

    public static AssetAdministrationShell GetAasFromDB()
{

    HttpClient client = new HttpClient();

    try
    {
        // Make a GET request to the API endpoint
        HttpResponseMessage response =
            client.GetAsync("http://localhost:8003/api/get_data/aas_collection/").Result;
        HttpResponseMessage response1 =
            client.GetAsync("http://localhost:8003/api/get_data/submodel_collection/").Result;
        // Check if the request was successful (status code 200-299)
        if (response.IsSuccessStatusCode
            && response1.IsSuccessStatusCode)
        {
            JsonSerializerSettings =
                (JsonSerializerSettings)new DependencyInjectionJsonSerializerSettings();
            // Read and print the response content
            string content = response.Content.ReadAsStringAsync().Result;
            string content1 = response1.Content.ReadAsStringAsync().Result;
            JArray array = JArray.Parse(content);
            JArray array1 = JArray.Parse(content1);
            foreach (JObject element in array.Children<JObject>())
            {
                // Remove the specified attribute from each JObject
                element.Remove("_id");

            }

            foreach (JObject element1 in array1.Children<JObject>())
            {
                // Remove the specified attribute from each JObject
                element1.Remove("_id");

            }

            var aasmodifieddata = array;
            var submodelData = array1;
            var Firstobject = aasmodifieddata[0];
           Console.WriteLine( Firstobject.GetType());
            

            var d = aasmodifieddata[0].ToString();
            var aasdesription = JsonConvert.DeserializeObject<dynamic>(d);
            var Submodels = JsonConvert.DeserializeObject<dynamic>(array1.ToString());
            foreach (var VARIABLE in aasdesription)
            {
                Console.WriteLine(VARIABLE);
                Console.WriteLine(VARIABLE.GetType());
                
            }
            // AssetAdministrationShell aaspisted = new AssetAdministrationShell()
            var aasid = aasdesription["idShort"];
            var identification = aasdesription["identification"];
            var asset = aasdesription["asset"];
            var administration = aasdesription["administration"];
            var Persisteddescription = aasdesription["description"];
            var endpoints = aasdesription["endpoints"];
            
            Identifier data = JsonConvert.DeserializeObject<Identifier>(identification.ToString());
            Asset asset0 = JsonConvert.DeserializeObject<Asset>(asset.ToString());
            AdministrativeInformation administrativeInformation =JsonConvert.DeserializeObject<AdministrativeInformation>(administration.ToString());
            //AssetAdministrationShell aas = new AssetAdministrationShell(aasid, dat);
            LangStringSet description = JsonConvert.DeserializeObject<LangStringSet>(Persisteddescription.ToString());

            AssetAdministrationShell aas = new AssetAdministrationShell(aasid.ToString(), data)
            {
                Asset = asset0,
                Administration = administrativeInformation
            };
            
            //Parsing the Submodels

            foreach (var dat in submodelData.Children<JObject>())
            {
               
                var id = dat["idShort"];
                var submodelElement = dat["submodelElements"];
                var identification2 = dat["identification"];
               
                
                identification = JsonConvert.DeserializeObject<Identifier>(identification2.ToString());
                try
                {
                    Submodel submodel = new Submodel(id.ToString(), identification)
                    {
                    };
                    submodel.SubmodelElements = new ElementContainer<ISubmodelElement>(submodel);
                    foreach (var VARIABLE in submodelElement.Children<JObject>())
                    {
                    
                            Console.WriteLine(VARIABLE);

                    }
                    aas.Submodels.Add(submodel);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }

            } 
           


            // Convert addressValue to JObject

            // Access the nested city property


        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    return null;

}
    public static Submodel GetTestSubmodel()
{
    var propertyValue = "TestFromInside";
    var i = 0;
    var y = 2.0;
    var testSubmodel = new Submodel("TestSubmodel", new BaSyxSubmodelIdentifier("TestSubmodel", "1.0.0"))
    {
        SubmodelElements =
        {
            new Property<string>("TestProperty1")
            {
                Set = (prop, val) => propertyValue = val,
                Get = prop => { return propertyValue + "_" + i++; }
            },
            new Property<string>("TestProperty2")
            {
                Set = (prop, val) => propertyValue = val,
                Get = prop => { return propertyValue + "_" + i++; }
            },
            new Property<int>("TestProperty3") { Set = (prop, val) => i = val, Get = prop => { return i++; } },
            new Property<double>("TestProperty4")
            {
                Set = (prop, val) => y = val, Get = prop => { return Math.Pow(y, i); }
            },
            new Property<string>("TestPropertyNull")
            {
                Set = (prop, val) => propertyValue = val, Get = prop => { return null; }
            },
            new Property<string>("TestPropertyNoSetter")
            {
                Set = null, Get = prop => { return "You can't change me!"; }
            },
            new Property<string>("TestValueChanged1", "InitialValue"),
            new SubmodelElementCollection("TestSubmodelElementCollection")
            {
                Value =
                {
                    new Property<string>("TestSubProperty1")
                    {
                        Set = (prop, val) => propertyValue = val,
                        Get = prop => { return propertyValue + "_" + i--; }
                    },
                    new Property<string>("TestSubProperty2")
                    {
                        Set = (prop, val) => propertyValue = val,
                        Get = prop => { return propertyValue + "_" + i--; }
                    },
                    new Property<int>("TestSubProperty3")
                    {
                        Set = (prop, val) => i = val, Get = prop => { return i--; }
                    },
                    new Property<double>("TestSubProperty4")
                    {
                        Set = (prop, val) => y = val, Get = prop => { return Math.Pow(y, i); }
                    },
                    new SubmodelElementCollection("MaintenanceOrderDescription")
                    {
                        Value =
                        {
                            new Property<string>("OrderID")
                            {
                                Set = (prop, val) => propertyValue = val,
                                Get = prop => { return propertyValue; }
                            },
                            new Property<string>("MachineID", "DMU80eVo1")
                            {
                                Set = (prop, val) => propertyValue = val,
                                Get = prop => { return propertyValue; }
                            },
                            new Property<string>("MaintenanceDescription", "250HMaintenance")
                            {
                                Set = (prop, val) => propertyValue = val,
                                Get = prop => { return propertyValue; }
                            },
                            new Property<string>("VenueOfMaintenance", "Achen")
                            {
                                Set = (prop, val) => propertyValue = val,
                                Get = prop => { return propertyValue; }
                            }
                        }
                    }
                }
            },
            new Operation("GetTime")
            {
                OutputVariables = new OperationVariableSet()
                {
                    new Property<string>("Date"),
                    new Property<string>("Time"),
                    new Property<string>("Ticks")
                },
                OnMethodCalled = (op, inArgs, inOutArgs, outArgs, cancellationToken) =>
                {
                    outArgs.Add(new Property<string>("Date")
                    {
                        Value = "Heute ist der " + DateTime.Now.Date.ToString()
                    });
                    outArgs.Add(new Property<string>("Time")
                    {
                        Value = "Es ist " + DateTime.Now.TimeOfDay.ToString() + " Uhr"
                    });
                    outArgs.Add(new Property<string>("Ticks")
                    {
                        Value = "Ticks: " + DateTime.Now.Ticks.ToString()
                    });
                    return new OperationResult(true);
                }
            },
            new Operation("Calculate")
            {
                Description =
                    new LangStringSet()
                    {
                        new LangString("DE",
                            "Taschenrechner mit simulierter langer Rechenzeit zum Testen von asynchronen Aufrufen"),
                        new LangString("EN",
                            "Calculator with simulated long-running computing time for testing asynchronous calls")
                    },
                InputVariables = new OperationVariableSet()
                {
                    new Property<string>("Expression")
                    {
                        Description = new LangStringSet()
                        {
                            new LangString("DE", "Ein mathematischer Ausdruck (z.B. 5*9)"),
                            new LangString("EN", "A mathematical expression (e.g. 5*9)")
                        }
                    },
                    new Property<int>("ComputingTime")
                    {
                        Description = new LangStringSet()
                        {
                            new LangString("DE", "Die Bearbeitungszeit in Millisekunden"),
                            new LangString("EN", "The computation time in milliseconds")
                        }
                    }
                },
                OutputVariables = new OperationVariableSet() { new Property<double>("Result") },
                OnMethodCalled = async (op, inArgs, inOutArgs, outArgs, cancellationToken) =>
                {
                    var expression = inArgs["Expression"]?.GetValue<string>();
                    var computingTime = inArgs["ComputingTime"]?.GetValue<int>();
                    inOutArgs["HierRein"]?.SetValue("DaWiederRaus");
                    if (computingTime.HasValue) await Task.Delay(computingTime.Value, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                        return new OperationResult(false,
                            new Message(MessageType.Information, "Cancellation was requested"));
                    var value = CalculateExpression(expression);
                    outArgs.Add(new Property<double>("Result", value));
                    return new OperationResult(true);
                }
            }
        }
    };
    testSubmodel.SubmodelElements["TestProperty4"].Cast<IProperty>().ValueChanged +=
        SimpleAssetAdministrationShell_ValueChanged;
    return testSubmodel;
}



    private static void SimpleAssetAdministrationShell_ValueChanged(object sender, ValueChangedArgs e)
{
    logger.Info($"Property {e.IdShort} changed to {e.Value}");
}

    public static double CalculateExpression(string expression)
{
    var columnName = "Evaluation";
    var dataTable = new System.Data.DataTable();
    var dataColumn = new System.Data.DataColumn(columnName, typeof(double), expression);
    dataTable.Columns.Add(dataColumn);
    dataTable.Rows.Add(0);
    return (double)dataTable.Rows[0][columnName];
}
    }
    }