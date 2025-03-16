using Microsoft.Extensions.Logging;
using System;

namespace FodyLogging.Console
{

    [Log(ignorePrivate:false)]
    public class CpiService
    {
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