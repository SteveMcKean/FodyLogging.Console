#nullable enable
using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FodyLogging.Console;

public static class LoggerFactoryProvider
{
    private static readonly ConcurrentDictionary<(Type, LogLevel), ILogger> LoggerCache = new();

    public static IServiceProvider? ServiceProvider { get; set; }

    public static ILogger CreateLogger(Type type, LogLevel logLevel = LogLevel.Debug)
    {
        if (ServiceProvider == null)
        {
            throw new InvalidOperationException("ServiceProvider is not initialized.");
        }

        return LoggerCache.GetOrAdd((type, logLevel), key =>
            {
                var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();

                // Configure the logger to respect the specified log level
                var logger = loggerFactory.CreateLogger(key.Item1);

                logger.LogDebug("Logger created for {ClassName} at level {LogLevel}", key.Item1.Name, key.Item2);

                return logger;
            });
    }
}