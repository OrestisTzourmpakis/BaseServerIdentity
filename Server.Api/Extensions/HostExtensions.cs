using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Server.Infrastructure.Persistence;
using Server.Api;

namespace Server.Api.Extensions
{
    public static class HostExtensions
    {
        public async static Task<IHost> MigrateDatabase<T>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var databaseContext = services.GetRequiredService<ServerDbContext>();
                var logger = services.GetRequiredService<ILogger<T>>();
                try
                {
                    logger.LogInformation("Migrating sql database.");
                    await databaseContext.Database.MigrateAsync();
                    await SeedDatabase.SeedDatabaseContext(services);
                }
                catch (SqlException ex)
                {
                    logger.LogError(ex, "An error occured while migrating the sql database");
                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        await Task.Delay(2000);
                        await MigrateDatabase<T>(host, retryForAvailability);
                    }
                }
            }
            return host;
        }
    }
}