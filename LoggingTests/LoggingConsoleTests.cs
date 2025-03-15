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

        public CpiServiceTests(LoggerFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void LogReceivedDimensionVariant_ShouldLogCorrectly()
        {
            // Arrange
            var service = new CpiService();

            var dimensionVariant = new CpiSkuDimensionVariant
                {
                    Sku = "123",
                    Dimension = "456",
                    Variant = "789"
                };

            // Act
            service.LogReceivedDimensionVariant(dimensionVariant);

            // Assert
            fixture.MockLogger.Received().Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>((v) => v.ToString().Contains("Entering CpiService.LogReceivedDimensionVariant")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>());
        }
    }
}