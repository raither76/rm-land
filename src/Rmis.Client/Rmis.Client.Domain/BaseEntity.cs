using System;

namespace Rmis.Client.Domain
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }

        public DateTimeOffset CreateDate { get; set; }

        public DateTimeOffset ModifyDate { get; set; }
    }
}