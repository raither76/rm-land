namespace Rmis.Domain
{
    public class Direction : BaseEntity
    {
        public string DisplayName { get; set; }
        
        public Station FromStation { get; set; }

        public Station ToStation { get; set; }
    }
}