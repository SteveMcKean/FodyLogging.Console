﻿using System;
using FodyLogging.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LoggingTests
{
    public class LoggerFixture : IDisposable
    {
        public ILogger<CpiService> MockLogger { get; }
        public IServiceProvider ServiceProvider { get; }

        public LoggerFixture()
        {
            // Create a mock logger using NSubstitute
            MockLogger = Substitute.For<ILogger<CpiService>>();

            // Set up the ServiceProvider with the mock logger
            ServiceProvider = new ServiceCollection()
                .AddLogging(builder =>
                    {
                        builder.ClearProviders();
                        builder.SetMinimumLevel(LogLevel.Debug);
                        builder.AddProvider(new MockLoggerProvider(MockLogger));
                    })
                .BuildServiceProvider();

            // Set the ServiceProvider in LoggerFactoryProvider
            LoggerFactoryProvider.ServiceProvider = ServiceProvider;
        }

        public void Dispose()
        {
            if (ServiceProvider is IDisposable disposableServiceProvider) 
                disposableServiceProvider.Dispose();

            LoggerFactoryProvider.ServiceProvider = null;
        }
    }
}