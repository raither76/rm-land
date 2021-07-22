namespace Rmis.OpenWeather
{
    public interface IOpenWeatherProvider
    {
        MainWeatherInfo GetWeatherInfoByCity(string city);
    }
}