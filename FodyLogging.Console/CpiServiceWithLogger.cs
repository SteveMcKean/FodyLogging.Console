using Microsoft.Extensions.Logging;

namespace FodyLogging.Console;

[Log(ignorePrivate: false)]
public class CpiServiceWithLogger
{
    private ILogger<CpiServiceWithLogger> logger;
     
    public CpiServiceWithLogger(ILogger<CpiServiceWithLogger> logger)
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