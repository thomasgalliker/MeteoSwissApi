using System;
using System.IO;
using System.Linq;
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

            //var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddDebug();
                //builder.AddSimpleConsole(c =>
                //{
                //    c.TimestampFormat = $"{dateTimeFormat.ShortDatePattern} {dateTimeFormat.LongTimePattern} ";
                //});
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
            dumpOptions.CustomInstanceFormatters.AddFormatter<Speed>(l => l.ToString());
            dumpOptions.CustomInstanceFormatters.AddFormatter<Pressure>(l => l.ToString());
            dumpOptions.CustomInstanceFormatters.AddFormatter<Angle>(l => l.ToString());

            {
                // Create weather service instance manually or resolve it from any dependency injection framework:
                var logger = loggerFactory.CreateLogger<MeteoSwissWeatherService>();
                IMeteoSwissWeatherServiceOptions weatherServiceConfiguration = new MeteoSwissWeatherServiceOptions
                {
                    VerboseLogging = false,
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
                    VerboseLogging = false,
                };
                ISwissMetNetService swissMetNetService = new SwissMetNetService(logger, options);

                var weatherStations = await swissMetNetService.GetWeatherStationsAsync();
                Console.WriteLine(ObjectDumper.Dump(weatherStations.Take(3), dumpOptions));
                Console.WriteLine("...");
                Console.WriteLine();

                var weatherStation = await swissMetNetService.GetWeatherStationAsync(stationCode: "CHZ");
                Console.WriteLine(ObjectDumper.Dump(weatherStation, dumpOptions));
                Console.WriteLine();

                var weatherStationMeasurement = await swissMetNetService.GetLatestMeasurementAsync(stationCode: "CHZ");
                Console.WriteLine(ObjectDumper.Dump(weatherStationMeasurement, dumpOptions));
                Console.WriteLine();

                var measurements = await swissMetNetService.GetLatestMeasurementsAsync(cacheExpiration: TimeSpan.FromMinutes(20));
                Console.WriteLine($"measurements={measurements.Count()}");
                Console.WriteLine();
            }
            {
                var logger = loggerFactory.CreateLogger<SlfDataService>();
                ISlfDataServiceOptions options = new SlfDataServiceOptions
                {
                    VerboseLogging = true,
                };
                ISlfDataService slfDataService = new SlfDataService(logger, options);

                var measurements = (await slfDataService.GetLatestMeasurementsAsync())
                    .OrderBy(m => m.Station.Code)
                    .ToArray();

                Console.WriteLine(ObjectDumper.Dump(measurements.Take(3), dumpOptions));
                Console.WriteLine("...");
                Console.WriteLine();

                var measurementTIT1 = await slfDataService.GetLatestMeasurementByStationCodeAsync("SMN", "*TIT1");
                Console.WriteLine(ObjectDumper.Dump(measurementTIT1, dumpOptions));
                Console.WriteLine();

                var measurementALI2 = await slfDataService.GetLatestMeasurementByStationCodeAsync("IMIS", "ALI2");
                Console.WriteLine(ObjectDumper.Dump(measurementALI2, dumpOptions));
                Console.WriteLine();
                
                var stationInfo = await slfDataService.GetStationInfoAsync("SMN", "*TIT1");
                Console.WriteLine(ObjectDumper.Dump(stationInfo, dumpOptions));
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
