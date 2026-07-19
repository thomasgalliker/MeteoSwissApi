namespace MeteoSwissApi
{
    public interface IWeatherIconMapping
    {
        Task<Stream> GetIconAsync(int iconId);
    }
}