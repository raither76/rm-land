using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rmis.Domain;
using Rmis.Persistence.Abstract;

namespace Rmis.Persistence
{
    public class RmisDbContext : DbContext, IRmisDbContext
    {
        public RmisDbContext()
        {
        }

        public RmisDbContext(DbContextOptions<RmisDbContext> options) : base(options)
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
                optionsBuilder.UseNpgsql("User ID=postgres;Password=12345;Server=localhost;Port=5432;Database=Rmis; Integrated Security=true;Pooling=true;");
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RmisDbContext).Assembly);
        }

        public IRmisRepository<Schedule> ScheduleRepository => new EfRepository<Schedule>(this, true);
        public IRmisRepository<Route> RouteRepository => new EfRepository<Route>(this, true);
        public IRmisRepository<Station> StationRepository => new EfRepository<Station>(this, true);
    }
}