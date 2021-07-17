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

        public IScheduleRepository ScheduleRepository => new ScheduleRepository(this);
        public IRouteRepository RouteRepository => new RouteRepository(this);
        public IRmisRepository<Station> StationRepository => new EfRepository<Station>(this, false);
        public IDirectionRepository DirectionRepository => new DirectionRepository(this);
        public IRmisRepository<Stop> StopRepository => new EfRepository<Stop>(this, false);
    }
}