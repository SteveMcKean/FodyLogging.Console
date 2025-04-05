using System;
using FodyLogging.Console;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LoggingTests
{
    /// <summary>
    /// Contains unit tests for the <see cref="LoggerFixture"/> class, ensuring its functionality
    /// and behavior when interacting with mock loggers and the service provider.
    /// </summary>
    /// <remarks>
    /// This class includes tests to validate the creation, retrieval, and disposal of mock loggers,
    /// as well as the proper cleanup of resources.
    /// </remarks>
    public class LoggerFixtureTests : IDisposable
    {
        private readonly LoggerFixture loggerFixture;

        public LoggerFixtureTests()
        {
            loggerFixture = new LoggerFixture();
        }

        [Fact]
        public void GetMockLogger_ShouldReturnMockLogger()
        {
            // Act
            var logger = loggerFixture.GetMockLogger<CpiService>();

            // Assert
            Assert.NotNull(logger);
            Assert.True(logger.IsEnabled(LogLevel.Information));
            Assert.True(logger.IsEnabled(LogLevel.Error));
        }

        [Fact]
        public void GetMockLogger_ShouldReturnSameLoggerForSameType()
        {
            // Act
            var logger1 = loggerFixture.GetMockLogger<CpiService>();
            var logger2 = loggerFixture.GetMockLogger<CpiService>();

            // Assert
            Assert.Same(logger1, logger2);
        }

        [Fact]
        public void GetMockLogger_ShouldReturnDifferentLoggerForDifferentTypes()
        {
            // Act
            var logger1 = loggerFixture.GetMockLogger<CpiService>();
            var logger2 = loggerFixture.GetMockLogger<Program>();

            // Assert
            Assert.NotSame(logger1, logger2);
        }

        [Fact]
        public void Dispose_ShouldClearServiceProviderAndLoggers()
        {
            // Act
            loggerFixture.Dispose();

            // Assert
            Assert.Null(LoggerFactoryProvider.ServiceProvider);
        }

        public void Dispose()
        {
            loggerFixture.Dispose();
        }
    }
}
