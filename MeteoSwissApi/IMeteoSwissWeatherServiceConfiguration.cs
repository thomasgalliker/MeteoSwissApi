namespace MeteoSwissApi
{
    public interface IMeteoSwissWeatherServiceConfiguration
    {
        string ApiEndpoint { get; }

        string Language { get; }

        bool VerboseLogging { get; }
    }
}