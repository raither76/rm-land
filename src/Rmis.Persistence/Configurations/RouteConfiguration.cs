using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rmis.Domain;

namespace Rmis.Persistence.Configurations
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.ToTable(nameof(Route));

            builder.HasOne(r => r.Direction)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}