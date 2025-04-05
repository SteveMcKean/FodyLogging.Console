using System;
using Microsoft.Extensions.Logging.Console;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BasicConsoleApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(x =>
                    {
                        x.ClearProviders();
                        x.AddConsole();
                    })
                
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<PaymentService>();
                })
                .Build();
            
            var paymentService = host.Services.GetRequiredService<PaymentService>();
            paymentService.CreatePayment("steve@smckean.com", 15.99m, 1234);

            Console.ReadLine();
        }
    }
}