using System;
using FodyLogging.Console;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace LoggingTests
{
    public class CpiServiceTests : IClassFixture<LoggerFixture>
    {
        private readonly LoggerFixture fixture;
        //private readonly ILogger<CpiService> mockLogger;

        public CpiServiceTests(LoggerFixture fixture)
        {
            this.fixture = fixture;
            //mockLogger = fixture.GetMockLogger<CpiService>();
        }

        [Fact]
        public void LogReceivedDimensionVariant_ShouldLogCorrectly()
        {
            // Arrange
            var mockLogger = fixture.GetMockLogger<CpiService>();
            var service = new CpiService(); // No need to inject logger manually

            var dimensionVariant = new CpiSkuDimensionVariant
                {
                    Sku = "123",
                    Dimension = "456",
                    Variant = "789"
                };

            // Act
            service.LogReceivedDimensionVariant(dimensionVariant);

            // Assert Entry Log
            mockLogger.Received().Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains("Entering CpiService.LogReceivedDimensionVariant")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>());

            // Assert Exit Log
            mockLogger.Received().Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains("Exiting CpiService.LogReceivedDimensionVariant")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>());
        }
    }

}