using System.Collections.Generic;

namespace Rmis.OpenWeather
{
    public class WeatherResponse
    {
        public MainWeatherInfo main { get; set; }

        public WindInfo wind { get; set; }

        public List<WeatherInfo> weather { get; set; }
    }
}