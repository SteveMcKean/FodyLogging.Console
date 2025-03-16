using System;
using System.Reflection;
using FodyLogging.Console;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace LoggingTests
{
    // Dummy class to simulate a target of the LogAttribute.

    public class LogAttributeUsingFixtureTests : IClassFixture<LoggerFixture>
    {
        private readonly LoggerFixture loggerFixture;

        public LogAttributeUsingFixtureTests(LoggerFixture loggerFixture)
        {
            this.loggerFixture = loggerFixture;
        }

        [Fact]
        public void OnEntry_LogsInformation()
        {
            // Arrange: Get the mock logger for DummyClass.
            var expectedLogger = loggerFixture.GetMockLogger<CpiService>();
            // Configure the mock to return true for Information logs.
            expectedLogger.IsEnabled(LogLevel.Information).Returns(true);
    
            var attribute = new LogAttribute();
            
            var methodInfo = typeof(CpiService).GetMethod(nameof(CpiService.LogReceivedDimensionVariant))!;
            object[] parameters = { 42, "hello" };

            attribute.Init(new CpiService(), methodInfo, parameters);
            // Clear any calls made during initialization (e.g., the Debug log call).
            expectedLogger.ClearReceivedCalls();

            // Act: Invoke the OnEntry method.
            attribute.OnEntry();

            // Assert: Verify that LogInformation was called with a message containing "Entering" and expected details.
            expectedLogger.Received().Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("Entering") &&
                                    o.ToString()!.Contains("CpiService") &&
                                    o.ToString()!.Contains("LogReceivedDimensionVariant")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>());
        }


        [Fact]
        public void OnExit_LogsInformation()
        {
            // Arrange: Get the mock logger for DummyClass.
            var expectedLogger = loggerFixture.GetMockLogger<CpiService>();
            expectedLogger.IsEnabled(LogLevel.Information).Returns(true);

            // Create and initialize the LogAttribute.
            var attribute = new LogAttribute();
            var methodInfo = typeof(CpiService).GetMethod(nameof(CpiService.LogReceivedDimensionVariant))!;
            
            object[] parameters = { 55, "world" };
            attribute.Init(new CpiService(), methodInfo, parameters);

            // Act: Call the OnExit method.
            attribute.OnExit();

            // Assert: Verify that LogInformation was called with a message containing "Exiting",
            // along with the class and method names.
            expectedLogger.Received().Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("Exiting") &&
                                     o.ToString()!.Contains(nameof(CpiService)) &&
                                     o.ToString()!.Contains(nameof(CpiService.LogReceivedDimensionVariant))),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>());
        }

        [Fact]
        public void OnException_LogsError()
        {
            // Arrange: Get the mock logger for DummyClass.
            var expectedLogger = loggerFixture.GetMockLogger<CpiService>();
            expectedLogger.IsEnabled(LogLevel.Error).Returns(true);

            // Create and initialize the LogAttribute.
            var attribute = new LogAttribute();
            
            var methodInfo = typeof(CpiService).GetMethod(nameof(CpiService.LogReceivedDimensionVariant))!;
            
            object[] parameters = { 99, "exception" };
            attribute.Init(new CpiService(), methodInfo, parameters);

            // Create an exception to pass.
            var exception = new InvalidOperationException("Test exception");

            // Act: Call the OnException method.
            attribute.OnException(exception);

            // Assert: Verify that LogError was called with the exception and a message containing "Exception"
            // along with the class and method names.
            expectedLogger.Received().Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("Exception") &&
                                     o.ToString()!.Contains(nameof(CpiService)) &&
                                     o.ToString()!.Contains(nameof(CpiService.LogReceivedDimensionVariant))),
                exception,
                Arg.Any<Func<object, Exception, string>>());
        }
    }
}
