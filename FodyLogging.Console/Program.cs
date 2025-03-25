using System;
using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // Required for ConfigurationBuilder extensions
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using SerilogTimings.Extensions;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;


namespace FodyLogging.Console
{
    internal class Program
    {
        public IHost AppHost { get; set; }
        
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Ensure Microsoft.Extensions.Configuration.FileExtensions is referenced.
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                .Build();

            // Step 2: Configure Serilog using the configuration
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // Read from configuration (Ensure Serilog.Settings.Configuration is added via NuGet)
                .Enrich.FromLogContext()
                .Destructure.ByTransforming<CpiSkuDimensionVariant>(variant => new
                {
                    variant.Sku,
                    variant.Dimension
                })
                    
                .CreateLogger();
            
            var serviceProvider = new ServiceCollection()
                .AddOptions()
                .Configure<DevelopmentInstanceSettings>(configuration.GetSection("DevelopmentInstanceSettings"))
                .AddLogging(builder =>
                    {
                        builder.ClearProviders(); // Optionally clear other providers if you only want log4net.
                        builder.SetMinimumLevel(LogLevel.Debug); // Set your desired minimum log level.
                        builder.AddLog4Net("CpiServiceLog4Net.config"); // This loads your log4net config.
                        // You can still add console logging if desired:
                        ///builder.AddConsole();
                        builder.AddSerilog(dispose:true);
                        
                    })
                .AddLogging(builder => 
                    builder.AddProvider(new CpiServiceLoggerProvider()))
                .BuildServiceProvider();
            
            LoggerFactoryProvider.ServiceProvider = serviceProvider;
            
            var appSettingsOptions = serviceProvider.GetRequiredService<IOptions<DevelopmentInstanceSettings>>();
            var appSettings = appSettingsOptions.Value;
            
            Log.Logger.Information("Loaded setting: {AppSettingsUseVirtualKeyboard} - AnotherSetting: {AppSettingsMeasuringDevice}", appSettings.UseVirtualKeyboard, appSettings.MeasuringDevice);


            var cpiLogger = serviceProvider.GetRequiredService<ILogger<CpiService>>();
            var service = new CpiService();

            using (cpiLogger.BeginTimedOperation("Handling new variant"))
            {
                 service.LogReceivedDimensionVariant(new CpiSkuDimensionVariant
                            {
                                Sku = "99999",
                                Dimension = "456",
                                Variant = "789"
                            });
            }
            
            var myLogger = Log.Logger;
            
            using (myLogger.TimeOperation("Processing new variant")) 
            {
                await Task.Delay(1000);
                service.LogReceivedDimensionVariant(new CpiSkuDimensionVariant
                    {
                        Sku = "99999",
                        Dimension = "456",
                        Variant = "789"
                    });
            }
            
            var variant = new CpiSkuDimensionVariant
                {
                    Sku = "12345",
                    Dimension = "456",
                    Variant = "789"
                };
                
            myLogger.Information("New variant data: {@Variant}", variant);
            
            System.Console.ReadLine();
            Log.CloseAndFlush();

        }
    }
}