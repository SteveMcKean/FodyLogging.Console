using System;
using FodyLogging.Console;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace LoggingTests;

public class CpiServiceLoggingTests : IClassFixture<LoggerFixture>
{
    private readonly LoggerFixture loggerFixture;

    public CpiServiceLoggingTests(LoggerFixture loggerFixture)
    {
        this.loggerFixture = loggerFixture;
    }

    [Fact]
    public void LogReceivedDimensionVariant_LogsEntryAndExit()
    {
        // Arrange: Get the mock logger for CpiService.
        var expectedLogger = loggerFixture.GetMockLogger<CpiService>();
        // Make sure logging is enabled.
        expectedLogger.IsEnabled(LogLevel.Information).Returns(true);
        expectedLogger.IsEnabled(LogLevel.Error).Returns(true);

        var service = new CpiService();
        var variant = new CpiSkuDimensionVariant();

        // Optionally, clear calls from logger creation.
        expectedLogger.ClearReceivedCalls();

        // Act: Invoke the method (which Fody should have woven with the [Log] attribute).
        var result = service.LogReceivedDimensionVariant(variant);

        // Assert: Verify that the "Entering" and "Exiting" logs were called.
        expectedLogger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Entering") &&
                                o.ToString()!.Contains("CpiService") &&
                                o.ToString()!.Contains("LogReceivedDimensionVariant")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());

        expectedLogger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Exiting") &&
                                o.ToString()!.Contains("CpiService") &&
                                o.ToString()!.Contains("LogReceivedDimensionVariant")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());

        Assert.True(result);
    }

}
    