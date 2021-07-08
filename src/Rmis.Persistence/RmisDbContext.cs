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
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("RmisDbContext");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        public IRmisRepository<Schedule> ScheduleRepository { get; }
        public IRmisRepository<Route> RouteRepository { get; }
        public IRmisRepository<Station> StationRepository { get; }
    }
}