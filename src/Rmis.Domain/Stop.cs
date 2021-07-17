namespace Rmis.Domain
{
    public class Stop : BaseEntity
    {
        public double Duration { get; set; }

        public Station Station { get; set; }

        public double? StopTime { get; set; }
    }
}