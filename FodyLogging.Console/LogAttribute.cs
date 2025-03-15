using MethodDecorator.Fody.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Text.Json;

namespace FodyLogging.Console
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LogAttribute : Attribute, IMethodDecorator
    {
        private ILogger? logger;
        
        private object[] parameters;
        private readonly bool shouldLog = true;
        private readonly object lockObj = new object();

        private string className = string.Empty;
        private string methodName = string.Empty;


        public bool IgnorePrivate { get; set; }

        public LogAttribute(bool ignorePrivate = true) => IgnorePrivate = ignorePrivate;

        public void Init(object instance, MethodBase method, object[] args)
        {
            className = method.DeclaringType?.Name ?? "UnknownClass";
            methodName = method.Name;
            parameters = args;

            try
            {
                logger = LoggerFactoryProvider.CreateLogger(method.DeclaringType ?? typeof(object));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Logger is null.", e);
            }
        }

        public void OnEntry()
        {
            if (!shouldLog)
                return;

            try
            {
                var serializedParams = SerializeParameters(parameters);

                lock (lockObj)
                {
                    if (serializedParams != null)
                    {
                        logger?.LogInformation("Entering {Class}.{Method} with parameters: {Parameters}", className, methodName, serializedParams);
                    }
                    else
                    {
                        logger?.LogInformation("Entering {Class}.{Method} with no parameters", className, methodName);
                    }
                }
            }
            catch (Exception ex)
            {
                lock (lockObj)
                {
                    logger?.LogError(ex, "Error logging entry for {Class}.{Method}", className, methodName);
                }
            }
        }

        public void OnExit()
        {
            if (!shouldLog)
                return;

            try
            {
                lock (lockObj)
                {
                    logger?.LogInformation("Exiting {Class}.{Method}", className, methodName);
                }
            }
            catch (Exception ex)
            {
                lock (lockObj)
                {
                    logger?.LogError(ex, "Error logging exit for {Class}.{Method}", className, methodName);
                }
            }
        }

        public void OnException(Exception exception)
        {
            if (!shouldLog)
                return;

            try
            {
                lock (lockObj)
                {
                    logger?.LogError(exception, "Exception in {Class}.{Method}", className, methodName);
                }
            }
            catch (Exception ex)
            {
                lock (lockObj)
                {
                    logger?.LogError(ex, "Error logging exception for {Class}.{Method}", className, methodName);
                }
            }
        }

        private static string SerializeParameters(object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return null;

            try
            {
                var paramStrings = new string[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    paramStrings[i] = JsonSerializer.Serialize(parameters[i]);
                }

                return string.Join(", ", paramStrings);
            }
            catch (Exception)
            {
                return string.Join(", ", parameters);
            }
        }
    }
}