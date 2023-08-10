using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UnitsNet;

namespace MeteoSwissApi.ConsoleSample
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Console.WriteLine($"MeteoSwissApi.ConsoleSample [Version 1.0.0.0]");
            Console.WriteLine($"(c)2023 superdev gmbh. All rights reserved.");
            Console.WriteLine();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddDebug();
            });

            var dumpOptions = new DumpOptions
            {
                DumpStyle = DumpStyle.Console,
                ExcludeProperties = { "Graph" },
            };
            dumpOptions.CustomInstanceFormatters.AddFormatter<Temperature>(t => t.ToString());
            dumpOptions.CustomInstanceFormatters.AddFormatter<Length>(l => l.ToString());
            dumpOptions.CustomInstanceFormatters.AddFormatter<RelativeHumidity>(l => l.ToString());
            dumpOptions.CustomInstanceFormatters.AddFormatter<Duration>(l => l.ToString());

            {
                // Create weather service instance manually or resolve it from any dependency injection framework:
                var logger = loggerFactory.CreateLogger<MeteoSwissWeatherService>();
                IMeteoSwissWeatherServiceOptions weatherServiceConfiguration = new MeteoSwissWeatherServiceOptions
                {
                    VerboseLogging = true,
                    Language = "en"
                };
                IMeteoSwissWeatherService weatherService = new MeteoSwissWeatherService(logger, weatherServiceConfiguration);

                // Request weather info:
                var weatherInfo = await weatherService.GetCurrentWeatherAsync(plz: 6330);
                Console.WriteLine(ObjectDumper.Dump(weatherInfo, dumpOptions));
                Console.WriteLine();

                var forecast = await weatherService.GetForecastAsync(plz: 6330);
                Console.WriteLine(ObjectDumper.Dump(forecast, dumpOptions));
                Console.WriteLine();
            }
            {
                var logger = loggerFactory.CreateLogger<SwissMetNetService>();
                ISwissMetNetServiceOptions options = new SwissMetNetServiceOptions
                {
                    VerboseLogging = true,
                };
                ISwissMetNetService swissMetNetService = new SwissMetNetService(logger, options);

                var weatherStations = await swissMetNetService.GetWeatherStationsAsync();
                Console.WriteLine(ObjectDumper.Dump(weatherStations, dumpOptions));
                Console.WriteLine();

                var measurements = await swissMetNetService.GetLatestMeasurementsAsync();
                Console.WriteLine(ObjectDumper.Dump(measurements, dumpOptions));
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to close this window...");
            Console.ReadKey();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"{e.ExceptionObject}");
        }
    }
}
