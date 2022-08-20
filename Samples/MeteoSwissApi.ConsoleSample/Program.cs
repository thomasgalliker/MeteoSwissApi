using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MeteoSwissApi.ConsoleSample
{
    public static class Program
    {
        private static IConfigurationRoot configuration;

        private static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Console.WriteLine($"MeteoSwissApi.ConsoleSample [Version 1.0.0.0]");
            Console.WriteLine($"(c) 2022 superdev gmbh. All rights reserved.");
            Console.WriteLine();

            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            // Create weather service instance manually or resolve it from any dependency injection framework:
            var logger = loggerFactory.CreateLogger<MeteoSwissWeatherService>();
            IMeteoSwissWeatherServiceConfiguration weatherServiceConfiguration = new MeteoSwissWeatherServiceConfiguration();
            IMeteoSwissWeatherService weatherService = new MeteoSwissWeatherService(logger, weatherServiceConfiguration);

            // Request weather info:
            const int plz = 6330;
            var weatherInfo = await weatherService.GetCurrentWeatherAsync(plz);

            Console.WriteLine();
            Console.WriteLine(ObjectDumper.Dump(weatherInfo.CurrentWeather));

            Console.WriteLine();
            Console.WriteLine(ObjectDumper.Dump(weatherInfo.Forecast));

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
