using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Infrastructure.Configurations.Roles;

namespace Server.Api
{
    public static class SeedDatabase
    {
        public async static Task SeedDatabaseContext(IServiceProvider serviceProvider)
        {
            await DataInitializer.SeedRoles(serviceProvider);
            await DataInitializer.SeedUsers(serviceProvider);
        }
    }
}