using System;
using System.Collections.Concurrent;
using FodyLogging.Console;
using LoggingTests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class LoggerFixture : IDisposable
{
    private readonly IServiceProvider serviceProvider;
    private readonly ConcurrentDictionary<string, ILogger> mockLoggers = new();

    public LoggerFixture()
    {
        // Create a MockLoggerProvider with the concurrent dictionary.
        var mockLoggerProvider = new MockLoggerProvider(mockLoggers);

        // Create a substitute ILoggerFactory that uses the mock logger provider.
        var mockLoggerFactory = Substitute.For<ILoggerFactory>();
        
        mockLoggerFactory.CreateLogger(Arg.Any<string>())
            .Returns(callInfo =>
            {
                var categoryName = callInfo.Arg<string>();
                var logger = mockLoggerProvider.CreateLogger(categoryName);
                
                // Configure the logger to return true for Information and Error log levels.
                logger.IsEnabled(LogLevel.Information).Returns(true);
                logger.IsEnabled(LogLevel.Error).Returns(true);
                
                return logger;
            });

        // Build a service provider with the mocked ILoggerFactory.
        serviceProvider = new ServiceCollection()
            .AddSingleton<ILoggerFactory>(mockLoggerFactory)
            .BuildServiceProvider();

        // Set the static ServiceProvider so that LoggerFactoryProvider can resolve the factory.
        LoggerFactoryProvider.ServiceProvider = serviceProvider;
    }

    public ILogger<T> GetMockLogger<T>() where T : class
    {
        var categoryName = typeof(T).FullName;
        
        
        return (ILogger<T>)mockLoggers.GetOrAdd(categoryName, _ =>
            {
                var logger = Substitute.For<ILogger<T>>();
                logger.IsEnabled(LogLevel.Information).Returns(true);
                logger.IsEnabled(LogLevel.Error).Returns(true);
                
                return logger;
            });
    }

    public void Dispose()
    {
        LoggerFactoryProvider.ServiceProvider = null;
        mockLoggers.Clear();
        
        if (serviceProvider is IDisposable disposableServiceProvider)
            disposableServiceProvider.Dispose();
    }
}
