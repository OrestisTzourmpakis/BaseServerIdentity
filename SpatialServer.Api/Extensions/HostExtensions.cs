using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpatialServer.Infrastructure.Persistence;

namespace SpatialServer.Api.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<T>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var databaseContext = services.GetRequiredService<SpatialDbContext>();
                var logger = services.GetRequiredService<ILogger<T>>();
                try
                {
                    logger.LogInformation("Migrating sql database.");
                    databaseContext.Database.Migrate();
                }
                catch (SqlException ex)
                {
                    logger.LogError(ex, "An error occured while migrating the sql database");
                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<T>(host, retryForAvailability);
                    }
                }
            }
            return host;
        }
    }
}