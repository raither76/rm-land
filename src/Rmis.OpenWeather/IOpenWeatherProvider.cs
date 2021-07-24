namespace Rmis.OpenWeather
{
    public interface IOpenWeatherProvider
    {
        WeatherResponse GetWeatherInfoByGeo(double latitude, double longitude);
    }
}