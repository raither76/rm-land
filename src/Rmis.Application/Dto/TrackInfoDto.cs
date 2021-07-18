namespace Rmis.Application
{
    public class TrackInfoDto
    {
        public string TrainNumber { get; init; }
        
        public double Latitude { get; init; }

        public double Longitude { get; init; }

        public double? Speed { get; init; }
    }
}