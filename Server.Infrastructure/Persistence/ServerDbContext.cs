using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Models;

namespace Server.Infrastructure.Persistence
{
    public enum Roles
    {
        Administrator,
        StoreOwner
    }
    public class ServerDbContext : IdentityDbContext<ApplicationUser>
    {
        public ServerDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ServerDbContext).Assembly);
            base.OnModelCreating(builder);
        }
        // when we add a user in the table!!
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IdentityUser>())
            {
                // has the email, username here!
                // entry.Entity.UserName = entry.Entity.UserName + "aaa";
                // entry.Entity.NormalizedUserName = entry.Entity.NormalizedUserName + "AAA";
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<Points> Points { get; set; }
        public DbSet<Store> Stores { get; set; }
    }
}