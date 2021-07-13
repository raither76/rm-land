using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using Rmis.Client.Application;
using Rmis.Client.Persistence;
using Rmis.Client.Persistence.Abstract;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Rmis.Client.WepApi
{
    public class Program
    {
 public static void Main(string[] args)
        {
            Logger logger = null;
            
            try
            {
                string environmentString = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                var config = GetApplicationConfig(environmentString);
                var loggingConfiguration = new NLogLoggingConfiguration(config.GetSection("NLog"));
                LogManager.Configuration = loggingConfiguration;
                logger = LogManager.GetCurrentClassLogger();
                
                logger.Debug("init main");
                
                string applicationVersion = typeof(Program).Assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion;
                
                logger.Debug($"Rmis version: {applicationVersion}");
                
                logger.Debug($"ASPNETCORE_ENVIRONMENT={environmentString}");
                
                var host = CreateWebHostBuilder(environmentString).Build();

                using (var scope = host.Services.CreateScope())
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetService<IRmisClientDbContext>();

                        var concreteContext = (RmisClientDbContext)context;
                        concreteContext.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Произошла ошибка при инициализации базы данных");
                        throw;
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                if(logger != null)
                    logger.Error(ex, "В процессе работа приложения были обнаружены исключительны ситуации. Работа приложения приостановлена");
                
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string environment)
        {
#if DEBUG
            string contentRootPath = Directory.GetCurrentDirectory();
#else
           string contentRootPath = AppDomain.CurrentDomain.BaseDirectory;
#endif
            var config = GetApplicationConfig(environment);
            
            
            return WebHost.CreateDefaultBuilder()
                .UseEnvironment(environment)
                .ConfigureServices(services =>
                {
                    services.AddControllers();
                    services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "RmisClient.WebApi", Version = "v1"}); });
                    services.AddRmisClientPersistence(config);
                    services.AddRmisClientApplication(config);
                })
                .UseConfiguration(config)
                .UseContentRoot(contentRootPath)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                
                .UseStartup<Startup>();
        }
            
        private static IConfigurationRoot GetApplicationConfig(string environment)
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();
        }
    }
}