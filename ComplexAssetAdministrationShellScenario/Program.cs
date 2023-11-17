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

using BaSyx.AAS.Server.Http;
using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.API.Components;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Models.Connectivity;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Registry.Client.Http;
using BaSyx.Registry.ReferenceImpl.FileBased;
using BaSyx.Registry.Server.Http;
using BaSyx.Submodel.Server.Http;
using BaSyx.Utils.Settings.Sections;
using BaSyx.Utils.Settings.Types;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ComplexAssetAdministrationShellScenario
{
    internal class Program
    {
        private static RegistryHttpClient registryClient;
        private static DataStorage mainDataStorage = new DataStorage();
        private static string pData;
        public static IConfiguration Configuration { get; set; }

        private static async Task Main(string[] args)
        {
            // Call the MQTT subscriber function

            // Wait for some time to receive messages (you can adjust this based on your requirements)
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables() // Allows overriding settings with environment variables
                .Build();
            

            await Task.Run(async () =>
            {
                // Call the MQTT subscriber function
                //await MqttPublisherAndReceiver.MqttSubscribeAsync(args[1], int.Parse(args[2]), args[3]);
                //await MqttPublisherAndReceiver.MqttSubscribeAsync(args[1], int.Parse(args[2]), args[3], mainDataStorage, pData);
                //string mqttBrokerAddress = Configuration["MQTT_BROKER_ADDRESS"];
                string BrokerAddress = Configuration["MES_APPLICATION_CONFIG:BROKER_ADDRESS"];
                int BrokerPort = Int32.Parse(Configuration["MES_APPLICATION_CONFIG:BROKER_PORT"]);
                string SubsciptionTopic = Configuration["MES_APPLICATION_CONFIG:SUBSCRIPTION_TOPIC"];
                Console.WriteLine(BrokerAddress);
                
                await MqttPublisherAndReceiver.MqttSubscribeAsync(BrokerAddress, BrokerPort, SubsciptionTopic, mainDataStorage,
                    pData);

                // Wait for some time to receive messages (you can adjust this based on your requirements)
                await Task.Delay(10);
                // Get the received messages from the subscriber
            });

            await Task.Delay(1);
            registryClient = new RegistryHttpClient();
            LoadScenario();

            Console.WriteLine("Press enter to quit...");
            Console.ReadKey();
        }

        private static void LoadScenario()
        {
            LoadRegistry();
            LoadSingleShell();
            LoadMultipleShells();
            LoadMultipleSubmodels();
        }

        public static void LoadMultipleSubmodels()
        {
            var submodelRepositorySettings = ServerSettings.CreateSettings();
            submodelRepositorySettings.ServerConfig.Hosting.ContentPath = "Content";
            submodelRepositorySettings.ServerConfig.Hosting.Urls.Add("http://+:6999");
            submodelRepositorySettings.ServerConfig.Hosting.Urls.Add("https://+:6499");

            var multiServer = new SubmodelRepositoryHttpServer(submodelRepositorySettings);
            // Create an instance of the MyPostHandler class
            var postHandler = new MesAasPostHandler();

            // Add the POST endpoint
            //Register the POST endpoint

            multiServer.WebHostBuilder.Configure(app =>
            {
                app.Map("/mes-notification",
                    builder =>
                    {
                        builder.Use(async (context, next) =>
                        {
                            await postHandler.HandlePostRequest(context, mainDataStorage);
                        });
                    });
            });


            multiServer.WebHostBuilder.UseNLog();
            var repositoryService = new SubmodelRepositoryServiceProvider();

            for (var i = 0; i < 3; i++)
            {
                var submodel = new Submodel("MultiSubmodel_" + i,
                    new BaSyxSubmodelIdentifier("MultiSubmodel_" + i, "1.0.0"))
                {
                    Description = new LangStringSet()
                    {
                        new LangString("de", i + ". Teilmodell"),
                        new LangString("en", i + ". Submodel")
                    },
                    Administration = new AdministrativeInformation()
                    {
                        Version = "1.0",
                        Revision = "120"
                    },
                    SubmodelElements = new ElementContainer<ISubmodelElement>()
                    {
                        new Property<string>("Property_" + i, "TestValue_" + i),
                        new SubmodelElementCollection("Coll_" + i)
                        {
                            Value =
                            {
                                new Property<string>("SubProperty_" + i, "TestSubValue_" + i)
                            }
                        }
                    }
                };

                var submodelServiceProvider = submodel.CreateServiceProvider();
                repositoryService.RegisterSubmodelServiceProvider(submodel.IdShort, submodelServiceProvider);
            }

            var endpoints =
                multiServer.Settings.ServerConfig.Hosting.Urls.ConvertAll(c =>
                    new HttpEndpoint(c.Replace("+", "127.0.0.1")));
            repositoryService.UseDefaultEndpointRegistration(endpoints);

            multiServer.SetServiceProvider(repositoryService);
            multiServer.ApplicationStopping = () =>
            {
                for (var i = 0; i < repositoryService.ServiceDescriptor.SubmodelDescriptors.Count(); i++)
                    registryClient.DeleteSubmodelRegistration(
                        new BaSyxShellIdentifier("MultiAAS_" + i, "1.0.0").ToIdentifier().Id,
                        repositoryService.ServiceDescriptor.SubmodelDescriptors[i].IdShort);
            };

            multiServer.AddBaSyxUI(PageNames.SubmodelRepositoryServer);
            multiServer.AddSwagger(Interface.SubmodelRepository);

            _ = multiServer.RunAsync();

            var shells =
                registryClient.RetrieveAllAssetAdministrationShellRegistrations(p =>
                    p.Identification.Id.Contains("SimpleAAS"));
            var shell = shells.Entity?.FirstOrDefault();
            for (var i = 0; i < repositoryService.ServiceDescriptor.SubmodelDescriptors.Count(); i++)
            {
                var descriptor = repositoryService.ServiceDescriptor.SubmodelDescriptors[i];
                registryClient.CreateOrUpdateSubmodelRegistration(
                    new BaSyxShellIdentifier("MultiAAS_" + i, "1.0.0").ToIdentifier().Id, descriptor.Identification.Id,
                    descriptor);

                if (shell != null)
                    registryClient.CreateOrUpdateSubmodelRegistration(shell.Identification.Id,
                        descriptor.Identification.Id, descriptor);
            }
        }


        private static void LoadMultipleShells()
        {
            var aasRepositorySettings = ServerSettings.CreateSettings();
            aasRepositorySettings.ServerConfig.Hosting.ContentPath = "Content";
            aasRepositorySettings.ServerConfig.Hosting.Urls.Add("http://+:5999");
            aasRepositorySettings.ServerConfig.Hosting.Urls.Add("https://+:5499");

            var multiServer = new AssetAdministrationShellRepositoryHttpServer(aasRepositorySettings);
            multiServer.WebHostBuilder.UseNLog();
            var repositoryService = new AssetAdministrationShellRepositoryServiceProvider();

            for (var i = 0; i < 3; i++)
            {
                var aas = new AssetAdministrationShell("MultiAAS_" + i,
                    new BaSyxShellIdentifier("MultiAAS_" + i, "1.0.0"))
                {
                    Description = new LangStringSet()
                    {
                        new LangString("de", i + ". VWS"),
                        new LangString("en", i + ". AAS")
                    },
                    Administration = new AdministrativeInformation()
                    {
                        Version = "1.0",
                        Revision = "120"
                    },
                    Asset = new Asset("Asset_" + i, new BaSyxAssetIdentifier("Asset_" + i, "1.0.0"))
                    {
                        Kind = AssetKind.Instance,
                        Description = new LangStringSet()
                        {
                            new LangString("de", i + ". Asset"),
                            new LangString("en", i + ". Asset")
                        }
                    }
                };

                aas.Submodels.Create(new Submodel("TestSubmodel", new BaSyxSubmodelIdentifier("TestSubmodel", "1.0.0"))
                {
                    SubmodelElements =
                    {
                        new Property<string>("Property_" + i, "TestValue_" + i),
                        new SubmodelElementCollection("Coll_" + i)
                        {
                            Value =
                            {
                                new Property<string>("SubProperty_" + i, "TestSubValue_" + i)
                            }
                        }
                    }
                });

                var aasServiceProvider = aas.CreateServiceProvider(true);
                repositoryService.RegisterAssetAdministrationShellServiceProvider(
                    new BaSyxShellIdentifier("MultiAAS_" + i, "1.0.0").ToIdentifier().Id, aasServiceProvider);
            }

            var endpoints =
                multiServer.Settings.ServerConfig.Hosting.Urls.ConvertAll(c =>
                    new HttpEndpoint(c.Replace("+", "127.0.0.1")));
            repositoryService.UseDefaultEndpointRegistration(endpoints);

            multiServer.SetServiceProvider(repositoryService);
            multiServer.ApplicationStopping = () =>
            {
                foreach (var aasDescriptor in repositoryService.ServiceDescriptor.AssetAdministrationShellDescriptors)
                    registryClient.DeleteAssetAdministrationShellRegistration(aasDescriptor.Identification.Id);
            };

            multiServer.AddBaSyxUI(PageNames.AssetAdministrationShellRepositoryServer);
            multiServer.AddSwagger(Interface.AssetAdministrationShellRepository);

            _ = multiServer.RunAsync();

            foreach (var aasDescriptor in repositoryService.ServiceDescriptor.AssetAdministrationShellDescriptors)
                registryClient.CreateOrUpdateAssetAdministrationShellRegistration(aasDescriptor.Identification.Id,
                    aasDescriptor);
        }

        private static void LoadSingleShell()
        {
            var aas = SimpleAssetAdministrationShell.SimpleAssetAdministrationShell.GetAssetAdministrationShell();
            var testSubmodel = aas.Submodels["TestSubmodel"];

            var submodelServerSettings = ServerSettings.CreateSettings();
            submodelServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            submodelServerSettings.ServerConfig.Hosting.Urls.Add("http://+:5222");
            submodelServerSettings.ServerConfig.Hosting.Urls.Add("https://+:5422");

            var submodelServer = new SubmodelHttpServer(submodelServerSettings);
            
            // Create an instance of the MyPostHandler class
         
            // Add the POST endpoint
            //Register the POST endpoint
            submodelServer.WebHostBuilder.UseNLog();
            var submodelServiceProvider = testSubmodel.CreateServiceProvider();
            submodelServer.SetServiceProvider(submodelServiceProvider);
            submodelServiceProvider.UseAutoEndpointRegistration(submodelServerSettings.ServerConfig);
            submodelServer.AddBaSyxUI(PageNames.SubmodelServer);
            submodelServer.AddSwagger(Interface.Submodel);
            _ = submodelServer.RunAsync();

            var aasServerSettings = ServerSettings.CreateSettings();
            aasServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            aasServerSettings.ServerConfig.Hosting.Urls.Add("http://+:5111");
            aasServerSettings.ServerConfig.Hosting.Urls.Add("https://+:5411");

            var aasServiceProvider = aas.CreateServiceProvider(true);
            aasServiceProvider.SubmodelRegistry.RegisterSubmodelServiceProvider(testSubmodel.IdShort,
                submodelServiceProvider);
            aasServiceProvider.UseAutoEndpointRegistration(aasServerSettings.ServerConfig);

            var aasServer = new AssetAdministrationShellHttpServer(aasServerSettings);
            aasServer.WebHostBuilder.UseNLog();
            aasServer.SetServiceProvider(aasServiceProvider);
            aasServer.ApplicationStopping = () =>
            {
                registryClient.DeleteAssetAdministrationShellRegistration(aas.Identification.Id);
            };
            aasServer.AddBaSyxUI(PageNames.AssetAdministrationShellServer);
            aasServer.AddSwagger(Interface.AssetAdministrationShell);
            _ = aasServer.RunAsync();

            registryClient.CreateOrUpdateAssetAdministrationShellRegistration(aas.Identification.Id,
                new AssetAdministrationShellDescriptor(aas, aasServiceProvider.ServiceDescriptor.Endpoints));
            registryClient.CreateOrUpdateSubmodelRegistration(aas.Identification.Id, testSubmodel.Identification.Id,
                new SubmodelDescriptor(testSubmodel, submodelServiceProvider.ServiceDescriptor.Endpoints));
        }

        private static void LoadRegistry()
        {
            var registrySettings = ServerSettings.CreateSettings();
            registrySettings.ServerConfig.Hosting = new HostingConfiguration()
            {
                Urls = new List<string>()
                {
                    "http://+:4999",
                    "https://+:4499"
                },
                Environment = "Development",
                ContentPath = "Content"
            };

            var registryServer = new RegistryHttpServer(registrySettings);
            registryServer.WebHostBuilder.UseNLog();
            var fileBasedRegistry = new FileBasedRegistry();
            registryServer.SetRegistryProvider(fileBasedRegistry);
            registryServer.AddBaSyxUI(PageNames.AssetAdministrationShellRegistryServer);
            registryServer.AddSwagger(Interface.AssetAdministrationShellRegistry);
            _ = registryServer.RunAsync();
        }
    }
}