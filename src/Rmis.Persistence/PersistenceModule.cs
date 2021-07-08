using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rmis.Persistence
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddRmisPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("RmisDbContext");
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<RmisDbContext>(options => options.UseNpgsql(connectionString));
            
            return services;
        }
    }
}