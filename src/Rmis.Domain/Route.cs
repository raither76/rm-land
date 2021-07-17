using System.Collections.Generic;

namespace Rmis.Domain
{
    public class Route : BaseEntity
    {
        public int Number { get; set; }

        public string YaId { get; set; }
        
        public Direction Direction { get; set; }

        public List<Stop> Stops { get; set; } = new List<Stop>();
    }
}