using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Console;
using PostSharp.Patterns.Diagnostics.Backends.Serilog;
using Serilog;

namespace PostSharp.Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.Console() // This requires the Serilog.Sinks.Console package
                //.WriteTo.File("logs/log.txt")
                .CreateLogger();

            LoggingServices.DefaultBackend = new SerilogLoggingBackend(Log.Logger);

            var service = new CpiService();
            service.LogReceivedDimensionVariant(new CpiSkuDimensionVariant
            {
                Sku = "123",
                Dimension = "456",
                Variant = "789"
            });

            System.Console.ReadLine();
        }
    }
}