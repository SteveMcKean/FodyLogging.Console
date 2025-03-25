using System;
using System.Diagnostics;
using log4net.Core;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace FodyLogging.Console;

public class TimedOperation: IDisposable
{
    private readonly ILogger logger;
    private readonly LogLevel logLevel;

    private string messageTemplate;
    private readonly object?[] args;
    private long startingTimeStamp;

    public TimedOperation(ILogger logger, LogLevel logLevel, string messageTemplate, object[] args)
    {
        this.logger = logger;
        this.logLevel = logLevel;
        this.messageTemplate = messageTemplate;
        
        this.args = new object[args.Length + 1];
        Array.Copy(this.args, args, args.Length);

        this.startingTimeStamp = Stopwatch.GetTimestamp();
    }

    public void Dispose()
    {
        // i needs to calculate the elapsed time in milliseconds
        var elapsedMs = (Stopwatch.GetTimestamp() - startingTimeStamp) * 1000 / Stopwatch.Frequency;
        args[args.Length - 1] = elapsedMs;
        
        logger.Log(logLevel, $"{messageTemplate} completed in {{OperationDurationMs}}ms", args);
        
    }
}