using System;
using Microsoft.Extensions.Logging;

namespace FodyLogging.Console
{

    [Log(ignorePrivate:false)]
    public class CpiService
    {
       // private ILogger<CpiService> logger;

        public CpiService()
        {
            //this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public bool LogReceivedDimensionVariant(CpiSkuDimensionVariant receivedDimensionVariant)
        {
            TestMethod();
            return true;
        }

        private void TestMethod()
        {
            
        }
    }
}