using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Rmis.Persistence
{
    public class RmisDbContext : DbContext
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
                optionsBuilder.UseNpgsql("User ID=postgres;Password=1234;Server=localhost;Port=5432;Database=testDb; Integrated Security=true;Pooling=true;");
            }
        }
    }
}