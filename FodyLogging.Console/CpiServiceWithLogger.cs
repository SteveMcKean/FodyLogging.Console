using FodyLogging.Console;
using Microsoft.Extensions.Logging;

[Log(ignorePrivate: false)]
public class CpiServiceWithLogger
{
    private ILogger<CpiService> logger;
     
    public CpiServiceWithLogger(ILogger<CpiService> logger)
    {
        this.logger = logger;
        
    }

    public bool LogReceivedDimensionVariant(CpiSkuDimensionVariant receivedDimensionVariant)
    {
        logger.LogInformation("Entering CpiService.LogReceivedDimensionVariant");

        TestMethod();
        return true;
    }

    private void TestMethod()
    {
        logger.LogInformation("Entering CpiService.TestMethod");

    }
}