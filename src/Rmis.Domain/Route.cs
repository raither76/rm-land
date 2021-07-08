namespace Rmis.Domain
{
    public class Route : BaseEntity
    {
        public int Number { get; set; }

        public int TrainNumber { get; set; }

        public Station FromStation { get; set; }

        public Station ToStation { get; set; }
    }
}