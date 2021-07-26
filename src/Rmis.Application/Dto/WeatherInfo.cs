namespace Rmis.Application
{
    public class WeatherInfo
    {
        public decimal Temperature { get; set; }

        public decimal WindSpeed { get; set; }

        public int WindDirectionDeg { get; set; }

        public string WeatherDescription { get; set; }
    }
}