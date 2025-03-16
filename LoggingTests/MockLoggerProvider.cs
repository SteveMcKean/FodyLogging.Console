using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace LoggingTests
{
    public class MockLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> mockLoggers;

        public MockLoggerProvider(ConcurrentDictionary<string, ILogger> mockLoggers)
        {
            this.mockLoggers = mockLoggers;
        }

        public ILogger CreateLogger(string categoryName)
        {
            // Always return a substitute logger for the category
            return mockLoggers.GetOrAdd(categoryName, _ => Substitute.For<ILogger>());
        }

        public void Dispose()
        {
            mockLoggers.Clear(); // Clear dictionary to release mock references
        }
    }

}