using System.Linq;
using log4net;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace FodyLogging.Console
{
    
    [PSerializable]
    public class LogAspect: OnMethodBoundaryAspect
    { 
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogAspect));

        public override void OnEntry(MethodExecutionArgs args)
        {
            var parameters = string.Join(", ", args.Method.GetParameters()
                .Select((p, i) => $"{p.Name}={args.Arguments[i]}"));

            Logger.Info($"Entering {args.Method.Name}({parameters})");
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Logger.Info($"Exiting {args.Method.Name}, returned {args.ReturnValue}");
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Logger.Error($"Exception in {args.Method.Name}: {args.Exception.Message}", args.Exception);
        }
    }
}