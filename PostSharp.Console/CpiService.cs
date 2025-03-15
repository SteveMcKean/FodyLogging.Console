using PostSharp.Patterns.Diagnostics;

namespace PostSharp.Console
{
    [Log]
    public class CpiService
    {
        public void LogReceivedDimensionVariant(CpiSkuDimensionVariant receivedDimensionVariant)
        {
            TestMethod();
        }

        private void TestMethod()
        {
            // Method implementation
        }
    }
}