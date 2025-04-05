using FodyLogging.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SerilogTimings.Extensions;

public class Program
{
    public IHost AppHost { get; set; }

    public static void Main(string[] args)
    {
        MainAsync(args).GetAwaiter().GetResult();
    }

    public static async Task MainAsync(string[] args, IServiceProvider serviceProvider = null)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Development.json", false, true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Destructure.ByTransforming<CpiSkuDimensionVariant>(variant => new
            {
                variant.Sku,
                variant.Dimension
            })
            .CreateLogger();

        serviceProvider ??= new ServiceCollection()
            .AddOptions()
            .Configure<DevelopmentInstanceSettings>(configuration.GetSection("DevelopmentInstanceSettings"))
            .AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddLog4Net("CpiServiceLog4Net.config");
                builder.AddSerilog(dispose: true);
            })
            .AddLogging(builder =>
                builder.AddProvider(new CpiServiceLoggerProvider()))
            .BuildServiceProvider();

        LoggerFactoryProvider.ServiceProvider = serviceProvider;

        var appSettingsOptions = serviceProvider.GetRequiredService<IOptions<DevelopmentInstanceSettings>>();
        var appSettings = appSettingsOptions.Value;

        Log.Logger.Information(
            "Loaded setting: {AppSettingsUseVirtualKeyboard} - AnotherSetting: {AppSettingsMeasuringDevice}",
            appSettings.UseVirtualKeyboard, appSettings.MeasuringDevice);

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
