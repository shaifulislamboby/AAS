using System;
using System.Collections.Generic;
using System.Reflection;
using BaSyx.AAS.Client.Http;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using Newtonsoft.Json;


namespace ComplexAssetAdministrationShellScenario
{
    public static class MaintenanceType
    {
        public static readonly string Maintenance1 = "Maintenance_1";
        public static readonly string Maintenance2 = "Maintenance_2";
        public static readonly string Maintenance3 = "Maintenance_3";
        public static readonly string Maintenance4 = "Maintenance_4";

        public static string GetMaintenanceType(int maintenanceThreshold)
        {
            return maintenanceThreshold switch
            {
                250 => Maintenance1,
                500 => Maintenance2,
                1000 => Maintenance3,
                2000 => Maintenance4,
                _ => "Invalid"
            };
        }
    }

    public static class MaintenanceActions
    {
        public static List<SubmodelElementCollection> InteractionElements ;
        private static AssetAdministrationShellHttpClient Client { get; set; }
        public static SubmodelElementCollection IValue { get; }

        public static void MaintenanceActionsInitialization(string url)
        {
            Client = new AssetAdministrationShellHttpClient(new Uri(url));
        }

        public static void UpdateMaintenanceOrderStatus(string machineName, string maintenanceType, string orderStatus)
        {
            IValue updatedValue = new ElementValue(orderStatus, typeof(string));
            try
            {
                var resp = Client.UpdateSubmodelElementValue("MaintenanceOrderHandlingSubmodel",
                    machineName + "/" + maintenanceType + "/" + "MaintenanceOrderStatus" + "/" + "ActualOrderStatus",
                    updatedValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void UpdateMaintenanceOrder(string machineName, string maintenanceType, dynamic data,
            string orderStatus)
        {
            IValue updatedValue = new ElementValue(orderStatus, typeof(string));
            try
            {
                var resp = Client.UpdateSubmodelElementValue("MaintenanceOrderHandlingSubmodel",
                    machineName + "/" + maintenanceType + "/" + "MaintenanceOrderStatus" + "/" + "ActualOrderStatus",
                    updatedValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void UpdateMaintenanceRecord(MaintenanceSerializer maintenanceData, string[] properties = null)
        {
            /* received data from MES ----> first time(response when the MES-AAS will request a maintenance)
             * {
        "conversationId": "DMU80eVo1_500_Maintenance1::1",        
        "MessageId": "2",
        "MachineName": "DMU80eVo1",
        "MaintenanceThreshold": 500,
        "PlannedMaintenanceStart": "2023-04-29T14:17:49.13",
        "PlannedMaintenanceEnd": "2023-04-30T08:10:21.13"
}
             */
            /*
             * Second time: after completion of maintenance task of the machine
             * {
        "conversationId": "DMU80eVo1_500_Maintenance1::1",        
        "MessageId": "3",
        "MachineName": "DMU80eVo1",
        "MaintenanceThreshold": 500,
        "ActualMaintenanceStart": "2023-04-29T14:17:49.13",
        "ActualMaintenanceEnd": "2023-04-30T08:10:21.13",
	     "MaintenanceDuration": 3600}
             */
            /*
            string[] properties =
            {
                "PlannedMaintenanceStart", "PlannedMaintenanceEnd", "ActualMaintenanceStart",
                "ActualMaintenanceEnd", "MaintenanceDuration"
            };*/

            if (properties == null)
            {
                properties = new string[] { "PlannedMaintenanceStart", "PlannedMaintenanceEnd" };
            }
            foreach (var variable in properties)
            {
                string modifiedVariable = variable;
                PropertyInfo propertyInfo = maintenanceData.GetType().GetProperty(modifiedVariable);
                object propertyValue = propertyInfo?.GetValue(maintenanceData);
                if (propertyValue == null)
                {
                    // Set default datetime as year 1000
                    if (variable == "MaintenanceDuration")
                    {
                        propertyValue = 00.00;
                    }
                    else
                    {
                        propertyValue = new DateTime(0001, 01, 01);
                    }
                }

                IValue updatedValue = new ElementValue(propertyValue);
                if (modifiedVariable == "MaintenanceDuration")
                {
                    modifiedVariable = "MaintenanceCompletionTime";
                }

                try
                {
                    var response = Client.UpdateSubmodelElementValue("MaintenanceOrderHandlingSubmodel",
                        string.Concat(maintenanceData.MachineName, "/",
                            MaintenanceType.GetMaintenanceType(maintenanceData.MaintenanceThreshold), "/",
                            "MaintenanceRecord/", modifiedVariable, "/"), updatedValue);
                    Console.WriteLine($"MES-AAS server response: {response}");
                    if (response.Success)
                    {
                        Console.WriteLine($"MES-AAS server has been updated for this record: {modifiedVariable}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        
        public static void UpdateMaintenanceRecordForCompletionMessage(MaintenanceData maintenanceData, string[] properties = null)
        {
            if (properties == null)
            {
                properties = new string[]
                    { "PlannedMaintenanceEnd","PlannedMaintenanceStart","MaintenanceThreshold", "ActualMaintenanceStart", "ActualMaintenanceEnd", "MaintenanceDuration","MaintenanceStaff","MaintenanceCosts" };
            }

            foreach (var variable in properties)
            {
                string modifiedVariable = variable;
                PropertyInfo propertyInfo = maintenanceData.GetType().GetProperty(modifiedVariable);
                object propertyValue = propertyInfo?.GetValue(maintenanceData);
                if (propertyValue == null)
               {
                   /*  // Set default datetime as year 1000
                    if (variable == "MaintenanceDuration")
                    {
                        propertyValue = 00.00;
                    }
                    else
                    {
                        propertyValue = new DateTime(0001, 01, 01);
                    }*/
                }

                IValue updatedValue = new ElementValue(propertyValue);
                if (modifiedVariable == "MaintenanceDuration")
                {
                    modifiedVariable = "MaintenanceCompletionTime";
                }

                try
                {
                    var response = Client.UpdateSubmodelElementValue("MaintenanceOrderHandlingSubmodel",
                        string.Concat(maintenanceData.MachineName, "/",
                            MaintenanceType.GetMaintenanceType(maintenanceData.MaintenanceThreshold), "/",
                            "MaintenanceRecord/", modifiedVariable, "/"), updatedValue);
                    Console.WriteLine($"MES-AAS server response: {response}");
                    if (response.Success)
                    {
                        Console.WriteLine($"MES-AAS server has been updated for this record: {modifiedVariable}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void UpdateMaintenanceHistoryCount(string maintenanceType)
        {
            var currentRecord = Client.RetrieveSubmodelElementValue("MaintenanceSubmodel",
                string.Concat(maintenanceType, "/", "MaintenanceHistory/MaintenaceCounter"));
            var updateRecord = Convert.ToInt64(currentRecord.Entity.Value) + 1;
            IValue updatedValue = new ElementValue(updateRecord, typeof(int));
            var updateDr = Client.UpdateSubmodelElementValue("MaintenanceSubmodel",
                string.Concat(maintenanceType, "/", "MaintenanceHistory/MaintenaceCounter"), updatedValue);
            if (updateDr.Success) Console.WriteLine("Record Update Successfully");
            else Console.WriteLine("Unable to update record");
        }

        public static List<SubmodelElementCollection> GetMaintenanceRecord(string machineName, string maintenanceType)
        {
            var currentElement = Client.RetrieveSubmodelElement("MaintenanceOrderHandlingSubmodel",
                string.Concat(machineName, "/", maintenanceType, "/", "MaintenanceRecord"));
            var resultJson = currentElement.Entity.ToJson();
            var subModelElementCollection = JsonConvert.DeserializeObject<SubmodelElementCollection>(resultJson);
            InteractionElements = new List<SubmodelElementCollection>();
           InteractionElements.Add(subModelElementCollection);
            return InteractionElements;
        }
        
    }
}