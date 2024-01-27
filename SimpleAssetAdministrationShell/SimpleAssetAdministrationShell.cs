/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Distribution License 1.0 which is available at
* https://www.eclipse.org/org/documents/edl-v10.html
*
* 
*******************************************************************************/
/*
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
using System.Threading.Tasks;

namespace SimpleAssetAdministrationShell
{
    public static class SimpleAssetAdministrationShell
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public static AssetAdministrationShell GetAssetAdministrationShell()
        {
            AssetAdministrationShell aas = new AssetAdministrationShell("SimpleAAS", new BaSyxShellIdentifier("SimpleAAS", "1.0.0"))
            {
                Description = new LangStringSet()
                {
                   new LangString("de-DE", "Einfache VWS"),
                   new LangString("en-US", "Simple AAS")
                },
                Administration = new AdministrativeInformation()
                {
                    Version = "1.0",
                    Revision = "120"
                },
                Asset = new Asset("SimpleAsset", new BaSyxAssetIdentifier("SimpleAsset", "1.0.0"))
                {
                    Kind = AssetKind.Instance,
                    Description = new LangStringSet()
                    {
                          new LangString("de-DE", "Einfaches Asset"),
                          new LangString("en-US", "Simple Asset")
                    }
                }
            };

            Submodel testSubmodel = GetTestSubmodel();

            aas.Submodels.Add(testSubmodel);

            return aas;
        }

        public static Submodel GetTestSubmodel()
        {
            string propertyValue = "TestFromInside";
            int i = 0;
            double y = 2.0;

            Submodel testSubmodel = new Submodel("TestSubmodel", new BaSyxSubmodelIdentifier("TestSubmodel", "1.0.0"))
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
                    new Property<int>("TestProperty3")
                    {
                        Set = (prop, val) => i = val,
                        Get = prop => { return i++; }
                    },
                    new Property<double>("TestProperty4")
                    {
                        Set = (prop, val) => y = val,
                        Get = prop => { return Math.Pow(y, i); }
                    },
                    new Property<string>("TestPropertyNull")
                    {
                        Set = (prop, val) => propertyValue = val,
                        Get = prop => { return null; }
                    },
                    new Property<string>("TestPropertyNoSetter")
                    {
                        Set = null,
                        Get = prop => { return "You can't change me!"; }
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
                                Set = (prop, val) => i = val,
                                Get = prop => { return i--; }
                            },
                            new Property<double>("TestSubProperty4")
                            {
                                Set = (prop, val) => y = val,
                                Get = prop => { return Math.Pow(y, i); }
                            },
                            new SubmodelElementCollection("MaintenanceOrderDescription")
                            {
                                Value =
                                {
                                     new Property<string>("OrderID")
                                    {
                                        Set =(prop, val) => propertyValue = val,
                                        Get = prop => {return propertyValue; }
                                    },
                                     new Property<string>("MachineID","DMU80eVo1")
                                    {
                                        Set =(prop, val) => propertyValue = val,
                                        Get = prop => {return propertyValue; }
                                    },new Property<string>("MaintenanceDescription","250HMaintenance")
                                    {
                                        Set =(prop, val) => propertyValue = val,
                                        Get = prop => {return propertyValue; }
                                    },
                                     new Property<string>("VenueOfMaintenance","Achen")
                                    {
                                        Set =(prop, val) => propertyValue = val,
                                        Get = prop => {return propertyValue; }
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
                            outArgs.Add(new Property<string>("Date") { Value = "Heute ist der " + DateTime.Now.Date.ToString() });
                            outArgs.Add(new Property<string>("Time") { Value = "Es ist " + DateTime.Now.TimeOfDay.ToString() + " Uhr" });
                            outArgs.Add(new Property<string>("Ticks") { Value = "Ticks: " + DateTime.Now.Ticks.ToString() });
                            return new OperationResult(true);
                        }
                    },
                    new Operation("Calculate")
                    {
                        Description = new LangStringSet()
                        {
                            new LangString("DE", "Taschenrechner mit simulierter langer Rechenzeit zum Testen von asynchronen Aufrufen"),
                            new LangString("EN", "Calculator with simulated long-running computing time for testing asynchronous calls")
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
                       OutputVariables = new OperationVariableSet()
                       {
                           new Property<double>("Result")
                       },
                       OnMethodCalled = async (op, inArgs, inOutArgs, outArgs, cancellationToken) =>
                       {
                           string expression = inArgs["Expression"]?.GetValue<string>();
                           int? computingTime = inArgs["ComputingTime"]?.GetValue<int>();

                           inOutArgs["HierRein"]?.SetValue("DaWiederRaus");

                           if(computingTime.HasValue)
                            await Task.Delay(computingTime.Value, cancellationToken);
                     
                           if(cancellationToken.IsCancellationRequested)
                               return new OperationResult(false, new Message(MessageType.Information, "Cancellation was requested"));

                           double value = CalulcateExpression(expression);

                           outArgs.Add(new Property<double>("Result", value));
                           return new OperationResult(true);
                       }
                    }

                }
            };

            testSubmodel.SubmodelElements["TestProperty4"].Cast<IProperty>().ValueChanged += SimpleAssetAdministrationShell_ValueChanged;

            


            return testSubmodel;
        }

        private static void SimpleAssetAdministrationShell_ValueChanged(object sender, ValueChangedArgs e)
        {
            logger.Info($"Property {e.IdShort} changed to {e.Value}");
        }

        public static double CalulcateExpression(string expression)
        {
            string columnName = "Evaluation";
            System.Data.DataTable dataTable = new System.Data.DataTable();
            System.Data.DataColumn dataColumn = new System.Data.DataColumn(columnName, typeof(double), expression);
            dataTable.Columns.Add(dataColumn);
            dataTable.Rows.Add(0);
            return (double)(dataTable.Rows[0][columnName]);
        }
    }
}
*/
/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Distribution License 1.0 which is available at
* https://www.eclipse.org/org/documents/edl-v10.html
*
* 
*****************************************************************************/

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
using System.Threading.Tasks;

namespace SimpleAssetAdministrationShell
{
    public static class SimpleAssetAdministrationShell
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public static AssetAdministrationShell GetAssetAdministrationShell()
        {
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
                            new SubmodelElementCollection("DMU80evo-2")
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
                            new SubmodelElementCollection("DMU80evo-3") { Value = { } },
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
                            }
                            
                        }
                    };
            return maintenanceOrderHandlingSubmodel;
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