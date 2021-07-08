using System;

namespace Rmis.Domain
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }

        public DateTimeOffset CreateDate { get; set; }
        
        public DateTimeOffset ModifyDate { get; set; }
    }
}