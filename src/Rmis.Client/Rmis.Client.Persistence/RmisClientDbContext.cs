using Microsoft.EntityFrameworkCore;
using Rmis.Client.Domain;
using Rmis.Client.Persistence.Abstract;

namespace Rmis.Client.Persistence
{
    public class RmisClientDbContext : DbContext, IRmisClientDbContext
    {
        public RmisClientDbContext()
        {
        }

        public RmisClientDbContext(DbContextOptions<RmisClientDbContext> options) : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // var configuration = new ConfigurationBuilder()
                //     .SetBasePath(Directory.GetCurrentDirectory())
                //     .AddJsonFile("appsettings.json")
                //     .Build();
                // var connectionString = configuration.GetConnectionString("RmisDbContext");
                optionsBuilder.UseNpgsql("User ID=postgres;Password=12345;Server=localhost;Port=5432;Database=RmisClient; Integrated Security=true;Pooling=true;");
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RmisClientDbContext).Assembly);
        }

        public IRmisClientRepository<Schedule> ScheduleRepository => new EfRepository<Schedule>(this, false);
    }
}