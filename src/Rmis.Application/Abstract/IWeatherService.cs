namespace Rmis.Application.Abstract
{
    public interface IWeatherService
    {
        void SyncWeather();

        WeatherInfo GetWeatherByCoords(double latitude, double longitude);
    }
}