using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FodyLogging.Console
{
    public static class LoggerFactoryProvider
    {
        private static readonly ConcurrentDictionary<Type, ILogger> LoggerCache = new();
        

        public static IServiceProvider? ServiceProvider { get; set; }

        public static ILogger CreateLogger(Type type, LogLevel logLevel = LogLevel.Debug)
        {
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider is not initialized.");
            }


            return LoggerCache.GetOrAdd(type, t =>
                {
                    ILoggerFactory loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger(t);

                    // Example: Log a message at a different level
                    logger.Log(logLevel, "Logger created for {ClassName} at level {LogLevel}", type.Name, logLevel);

                    return logger;
                });
        }
    }
}