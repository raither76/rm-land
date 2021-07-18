using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rmis.Domain;

namespace Rmis.Persistence.Configurations
{
    public class TrackInfoConfiguration : IEntityTypeConfiguration<TrackInfo>
    {
        public void Configure(EntityTypeBuilder<TrackInfo> builder)
        {
            builder.ToTable(nameof(TrackInfo));

            builder.Property(p => p.Date).IsRequired();
            builder.Property(p => p.Latitude).IsRequired();
            builder.Property(p => p.Longitude).IsRequired();
            builder.Property(p => p.TrainNumber).IsRequired();
        }
    }
}