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

        private string className = string.Empty;
        private string methodName = string.Empty;

        public bool IgnorePrivate { get; set; }

        public LogAttribute(bool ignorePrivate = true) => IgnorePrivate = ignorePrivate;

        public void Init(object instance, MethodBase method, object[] args)
        {
            className = method.DeclaringType?.Name ?? "UnknownClass";
            methodName = method.Name;
            parameters = args;

            logger = LoggerFactoryProvider.CreateLogger(method.DeclaringType ?? typeof(object));
        }

        public void OnEntry()
        {
            if (!ShouldLog(LogLevel.Information))
                return;

            var serializedParams = SerializeParameters(parameters);
            logger?.LogInformation("Entering {Class}.{Method} with parameters: {Parameters}", className, methodName, serializedParams);
        }

        public void OnExit()
        {
            if (!ShouldLog(LogLevel.Information))
                return;

            logger?.LogInformation("Exiting {Class}.{Method}", className, methodName);
        }

        public void OnException(Exception exception)
        {
            if (!ShouldLog(LogLevel.Error))
                return;

            logger?.LogError(exception, "Exception in {Class}.{Method}", className, methodName);
        }

        private bool ShouldLog(LogLevel level)
        {
            return logger?.IsEnabled(level) ?? false;
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false,
            IgnoreNullValues = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };

        private static string SerializeParameters(object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return string.Empty;

            try
            {
                return JsonSerializer.Serialize(parameters, JsonOptions);
            }
            catch (Exception)
            {
                return string.Join(", ", parameters);
            }
        }
    }
}
