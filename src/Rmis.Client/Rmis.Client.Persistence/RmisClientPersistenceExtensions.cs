using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rmis.Client.Persistence.Abstract;

namespace Rmis.Client.Persistence
{
    public static class RmisClientPersistenceExtensions
    {
        public static IServiceCollection AddRmisClientPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("RmisClientDbContext");
            return services.AddEntityFrameworkNpgsql()
                .AddDbContext<RmisClientDbContext>(options => options.UseNpgsql(connectionString))
                .AddScoped<IRmisClientDbContext, RmisClientDbContext>();
        }
    }
}