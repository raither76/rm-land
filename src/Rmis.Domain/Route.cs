﻿namespace Rmis.Domain
{
    public class Route : BaseEntity
    {
        public int Number { get; set; }

        public string TrainNumber { get; set; }

        public Direction Direction { get; set; }
    }
}