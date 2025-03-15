using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;


namespace FodyLogging.Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Seq("http://localhost:5431") 
                .Enrich.FromLogContext()
                .Enrich.WithThreadName()// Ensure your Seq server is running at this URL.
                .Enrich.WithMachineName()
                .CreateLogger();

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                    {
                        builder.ClearProviders(); // Optionally clear other providers if you only want log4net.
                        builder.SetMinimumLevel(LogLevel.Debug); // Set your desired minimum log level.
                        builder.AddLog4Net("CpiServiceLog4Net.config"); // This loads your log4net config.
                        // You can still add console logging if desired:
                        builder.AddConsole();
                        builder.AddSerilog(dispose:true);
                    })
                .BuildServiceProvider();
            
            LoggerFactoryProvider.ServiceProvider = serviceProvider;

            //var logger = serviceProvider.GetRequiredService<ILogger<CpiService>>();
            var service = new CpiService();


            service.LogReceivedDimensionVariant(new CpiSkuDimensionVariant
            {
                Sku = "99999",
                Dimension = "456",
                Variant = "789"
            });
            
            System.Console.ReadLine();
            
        }
    }
}