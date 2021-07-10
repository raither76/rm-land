using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rmis.Domain;

namespace Rmis.Persistence.Configurations
{
    public class DirectionConfiguration : IEntityTypeConfiguration<Direction>
    {
        public void Configure(EntityTypeBuilder<Direction> builder)
        {
            builder.ToTable(nameof(Direction));

            builder.HasOne(d => d.FromStation)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(d => d.ToStation)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}