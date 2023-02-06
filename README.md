# MeteoSwissApi
[![Version](https://img.shields.io/nuget/v/MeteoSwissApi.svg)](https://www.nuget.org/packages/MeteoSwissApi)  [![Downloads](https://img.shields.io/nuget/dt/MeteoSwissApi.svg)](https://www.nuget.org/packages/MeteoSwissApi)

.NET client for easy access of Swiss national weather data. This library is a private, non-commercial project and is by no means related to or maintained by the Federal Office of Meteorology and Climatology MeteoSwiss.

### Download and Install MeteoSwissApi
This library is available on NuGet: https://www.nuget.org/packages/MeteoSwissApi
Use the following command to install MeteoSwissApi using NuGet package manager console:

    PM> Install-Package MeteoSwissApi

You can use this library in any .NET project which is compatible to .NET Standard 2.0 and higher.

### API Usage
The following sections document basic use cases of this library. The following code excerpts can also be found in the [sample applications](https://github.com/thomasgalliker/MeteoSwissApi/tree/develop/Samples).

#### Request weather info by PLZ
`MeteoSwissWeatherService` is the main entry point of this library. Create an instance of `MeteoSwissWeatherService` or inject `IMeteoSwissWeatherService` using dependency injection techniques.
```C#
IMeteoSwissWeatherService weatherService = new MeteoSwissWeatherService(logger, weatherServiceConfiguration);
```
Request weather information for any Swiss zip code (plz):
```C#
var weatherInfo = await weatherService.GetCurrentWeatherAsync(plz: 6330);
```

### Links
Federal Office of Meteorology and Climatology MeteoSwiss
https://www.meteoschweiz.admin.ch

### Contribution
Contributors welcome! If you find a bug or you want to propose a new feature, feel free to do so by opening a new issue on github.com.

### License
Check the terms and conditions of the Federal Office of Meteorology and Climatology MeteoSwiss before using this client library.

### Links
- https://www.meteoswiss.admin.ch
- https://www.meteoschweiz.admin.ch/wetter/gefahren/erlaeuterungen-der-gefahrenstufen.html
- https://github.com/deMynchi/ioBroker.meteoswiss/blob/779a93c0bf767c2227dc55e455e308744964eb99/src/main.ts
- 
- 