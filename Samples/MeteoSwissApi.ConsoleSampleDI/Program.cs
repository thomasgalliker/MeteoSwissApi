using System;
using System.Linq;
using System.Threading.Tasks;
using MeteoSwissApi;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static async Task Main(string[] _)
    {
        Console.WriteLine($"MeteoSwissApi.ConsoleSampleDI [Version 1.0.0.0]");
        Console.WriteLine($"(c)2023 superdev gmbh. All rights reserved.");
        Console.WriteLine();

        // Create DI container and register services
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();

        // Use code-based configuration:
        serviceCollection.AddMeteoSwissApi(o =>
        {
            o.VerboseLogging = false;
            o.Language = "de";
            o.SwissMetNet.CacheExpiration = TimeSpan.FromMinutes(20);
        });

        // Use configuration from appSettings.json:
        //var configuration = new ConfigurationBuilder()
        //    .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.json", true, true)
        //    .Build();

        //var configurationSection = configuration.GetSection("MeteoSwissApi");
        //serviceCollection.AddMeteoSwissApi(configurationSection);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Resolve services from DI container
        var meteoSwissWeatherService = serviceProvider.GetRequiredService<IMeteoSwissWeatherService>();
        var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(plz: 6330);
        Console.WriteLine("IMeteoSwissWeatherService.GetCurrentWeatherAsync");
        Console.WriteLine($"weatherInfo.CurrentWeather.Time: {weatherInfo.CurrentWeather.Time:u}");
        Console.WriteLine($"weatherInfo.CurrentWeather.Temperature: {weatherInfo.CurrentWeather.Temperature}");
        Console.WriteLine("...");
        Console.WriteLine();

        var swissMetNetService = serviceProvider.GetRequiredService<ISwissMetNetService>();
        var weatherStations = await swissMetNetService.GetWeatherStationsAsync();
        Console.WriteLine("ISwissMetNetService.GetWeatherStationsAsync");
        foreach (var weatherStation in weatherStations.Take(3))
        {
            Console.WriteLine(weatherStation.StationCode);
        }
        Console.WriteLine("...");
        Console.WriteLine();

        var slfDataService = serviceProvider.GetRequiredService<ISlfDataService>();
        var measurementTIT1 = await slfDataService.GetLatestMeasurementByStationCodeAsync("SMN", "*TIT1");
        Console.WriteLine("ISlfDataService.GetLatestMeasurementByStationCodeAsync");
        Console.WriteLine($"measurementTIT1.AirTemperature.Date: {measurementTIT1.AirTemperature.Date:u}");
        Console.WriteLine($"measurementTIT1.AirTemperature.Value: {measurementTIT1.AirTemperature.Value}");
        Console.WriteLine($"measurementTIT1.SnowHeight: {measurementTIT1.SnowHeight}");
        Console.WriteLine("...");
        Console.WriteLine();
    }
}