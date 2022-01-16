using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Server.Domain.Models;
using persistence = Server.Infrastructure.Persistence;

namespace Server.Infrastructure.Configurations.Roles
{
    public static class DataInitializer
    {

        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                foreach (var role in Enum.GetNames(typeof(persistence.Roles)))
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

            }
        }

        public static async Task SeedUsers(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                // create 2 users! with 2 roles!!!
                // admin with username admin
                // storeOwnerDemo
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "otzurbakis13@gmail.com"
                };
                var companyOwnerUser = new ApplicationUser
                {
                    UserName = "companyOwnerTest1",
                    Email = "o.tzourmpakis@gmail.com"
                };
                await SeedUsersHelper(userManager, adminUser, "Orestis123!", nameof(persistence.Roles.Administrator));
                await SeedUsersHelper(userManager, companyOwnerUser, "Orestis123!", nameof(persistence.Roles.CompanyOwner));
                // var checkAdmin = await userManager.FindByEmailAsync(adminUser.Email);
                // var checkStoreOwner = await userManager.FindByEmailAsync(storeOwnerUser.Email);
                // if (checkAdmin == null)
                // {
                //     var adminResult = await userManager.CreateAsync(adminUser, "Orestis123!");
                //     if (adminResult.Succeeded)
                //         await userManager.AddToRoleAsync(adminUser, nameof(SpatialServer.Infrastructure.Persistence.Roles.Administrator));
                // }
                // if (checkStoreOwner == null)
                //     await userManager.CreateAsync(storeOwnerUser, "Orestis123!");
            }
        }
        private static async Task SeedUsersHelper(UserManager<ApplicationUser> userManager, ApplicationUser user, string password, string role)
        {
            var checkUser = await userManager.FindByEmailAsync(user.Email);
            if (checkUser == null)
            {
                var createResult = await userManager.CreateAsync(user, password);
                if (createResult.Succeeded)
                    await userManager.AddToRoleAsync(user, role);
            }
        }

    }
}