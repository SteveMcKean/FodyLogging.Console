using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Extensions.Logging;

namespace FodyLogging.Console;

public class CpiServiceLoggerProvider: ILoggerProvider
{
    public void Dispose()
    {
       
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new CpiServiceLogger();
    }
}

public class CpiServiceLogger : ILogger
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, 
        Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;
        
        System.Console.WriteLine($"[{logLevel}] {formatter(state, exception)}");
        
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return NullScope.Instance;
    }
    
    internal sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        public void Dispose()
        {
        }
    }
    
}