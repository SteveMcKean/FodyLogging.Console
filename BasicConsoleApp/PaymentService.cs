using System;
using System.Data;
using Microsoft.Extensions.Logging;

namespace BasicConsoleApp
{
    public class PaymentService
    {
        private readonly ILogger<PaymentService> logger;
        //
        // private static readonly Action<ILogger, string, decimal, int, Exception> logCreatePayment =
        //     LoggerMessage.Define<string, decimal, int>(
        //         LogLevel.Information,
        //         new EventId(1, nameof(CreatePayment)),
        //         "Creating payment for {Email} with amount {Amount} for product {ProductId}");
        
        public PaymentService(ILogger<PaymentService> logger)
        {
            this.logger = logger;
        }
        
        public void CreatePayment(string email, decimal amount, int productId)
        {
            //logCreatePayment(logger,email, amount, productId, null);
            logger.LogPaymentCreation(email, amount, productId);
        }
    }
}