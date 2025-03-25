using System.IO.Pipelines;
using SerilogTimings;

namespace FodyLogging.Console;

public class DevelopmentInstanceSettings
{
    public string MeasuringDevice { get; set; }
    public string MeasuringDeviceConnectionString { get; set; }
    public bool UseVirtualKeyboard { get; set; }
    public bool AllowMultipleInstance { get; set; }
}