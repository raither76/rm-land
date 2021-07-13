using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rmis.Client.Domain;

namespace Rmis.Client.Persistence.Configurations
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable(nameof(Schedule));

            builder.Property(p => p.From).IsRequired();
            builder.Property(p => p.To).IsRequired();
            builder.Property(p => p.RouteNumber).IsRequired();
            builder.Property(p => p.TrainNumber).IsRequired();
        }
    }
}