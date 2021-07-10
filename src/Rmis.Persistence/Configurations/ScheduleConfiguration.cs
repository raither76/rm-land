using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rmis.Domain;

namespace Rmis.Persistence.Configurations
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable(nameof(Schedule));

            builder.HasOne(s => s.Route)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            Schedule s;
            builder.HasIndex(nameof(s.Date), nameof(Route) + "Id")
                .IsUnique();
        }
    }
}