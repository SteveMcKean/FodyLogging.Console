using MethodDecorator.Fody.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Text.Json;

namespace FodyLogging.Console
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LogMethodAttribute : Attribute, IMethodDecorator
    {
        private ILogger logger;
        private string methodName;
        private object[] parameters;
        private bool shouldLog = true;
        private readonly object lockObj = new object();

        public bool IgnorePrivate { get; set; }

        public LogMethodAttribute(bool ignorePrivate = true)
        {
            IgnorePrivate = ignorePrivate;
        }

        public void Init(object instance, MethodBase method, object[] args)
        {
            if (method.IsConstructor)
            {
                return;
            }

            if (IgnorePrivate && method.IsPrivate)
            {
                shouldLog = false;
                return;
            }

            shouldLog = true;

            try
            {
                logger = (ILogger)instance.GetType().GetField("logger",
                    BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(instance);

                methodName = method.Name;
                parameters = args;

                if (logger == null)
                {
                    throw new InvalidOperationException("Logger is null.");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error inside Init");
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
                        logger?.LogInformation("Entering {Method} with parameters: {Parameters}", methodName, serializedParams);
                    }
                    else
                    {
                        logger?.LogInformation("Entering {Method} with no parameters", methodName);
                    }
                }
            }
            catch (Exception ex)
            {
                lock (lockObj)
                {
                    logger?.LogError(ex, "Error logging entry for {Method}", methodName);
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
                    logger?.LogInformation("Exiting {Method}", methodName);
                }
            }
            catch (Exception ex)
            {
                lock (lockObj)
                {
                    logger?.LogError(ex, "Error logging exit for {Method}", methodName);
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
                    logger?.LogError(exception, "Exception in {Method}", methodName);
                }
            }
            catch (Exception ex)
            {
                lock (lockObj)
                {
                    logger?.LogError(ex, "Error logging exception for {Method}", methodName);
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