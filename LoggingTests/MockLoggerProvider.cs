using Microsoft.Extensions.Logging;

namespace LoggingTests
{
    public class MockLoggerProvider : ILoggerProvider
    {
        private readonly ILogger logger;

        public MockLoggerProvider(ILogger logger)
        {
            this.logger = logger;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return logger;
        }

        public void Dispose()
        {
        }
    }
}