using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using Timer = System.Threading.Timer;

namespace HelloAssetAdministrationShell.Services
{
    public class GetDataService : BackgroundService
    {

        private int executionCount = 0;
        private readonly ILogger<GetDataService> _logger;

        private Timer _timer = null;


        public GetDataService(ILogger<GetDataService>logger)
        {
            _logger = logger;
        }
        public IWebHostEnvironment WebHostEnvironment { get; set; }
        public GetDataService(IWebHostEnvironment webHostEnvironment) {

            WebHostEnvironment = webHostEnvironment;
            
        }

      
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) { 
            Console.WriteLine("Backgroun Service is running");
            
            await Task.Delay(1000);
            }
        }
    }
}
