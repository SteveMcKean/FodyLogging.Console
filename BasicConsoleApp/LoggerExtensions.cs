using Microsoft.Extensions.Logging;

namespace BasicConsoleApp
{
    public static partial class LoggerExtensions
    {
        [LoggerMessage(Level = LogLevel.Information,
            EventId = 1,
            Message = "Creating payment for {Email} with amount {Amount} for product {ProductId}")]
        public static void LogPaymentCreation(this ILogger logger, string email, decimal amount, int productId)
        {
            
        }

    }
}