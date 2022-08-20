using System;
using System.Diagnostics;

namespace MeteoSwissApi.Logging
{
    public class DebugLogger : ILogger
    {
        public void Log(LogLevel logLevel, string message)
        {
            Debug.WriteLine($"{DateTime.UtcNow}|MeteoSwissApi|{logLevel}|{message}[EOL]");
        }
    }
}