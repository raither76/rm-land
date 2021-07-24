namespace Rmis.Domain
{
    public class Station : BaseEntity
    {
        public string YaCode { get; set; }

        public string DisplayName { get; set; }

        public decimal TemperatureC { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public decimal WindSpeed { get; set; }

        public int WindDirectionDeg { get; set; }

        public string WeatherDescription { get; set; }
    }
}