using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rmis.Persistence.Abstract;

namespace Rmis.Persistence
{
    public static class RmisPersistenceExtensions
    {
        public static IServiceCollection AddRmisPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("RmisDbContext");
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<RmisDbContext>((provider, options) => options
                    .UseInternalServiceProvider(provider)
                    .UseNpgsql(connectionString));

            services.AddScoped<IRmisDbContext, RmisDbContext>();
            
            return services;
        }
    }
}