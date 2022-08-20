using System;
using MeteoSwissApi.Logging;

namespace MeteoSwissApi.ConsoleSample.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Log(LogLevel level, string message)
        {
            Console.WriteLine($"{DateTime.Now}|{level}|{message}");
        }
    }
}