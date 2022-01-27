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

    public class ServerDbContext : IdentityDbContext<ApplicationUser>
    {
        public ServerDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ServerDbContext).Assembly);
            //     builder.Entity<ApplicationUserCompany>()
            //     .HasKey(ac => new { ac.ApplicationUserId, ac.CompanyId });
            //     builder.Entity<ApplicationUserCompany>()
            //     .HasOne(ac => ac.ApplicationUser)
            //     .WithMany(a => a.ApplicationUserCompanies)
            //     .HasForeignKey(ac => ac.ApplicationUserId);
            //     builder.Entity<ApplicationUserCompany>()
            //    .HasOne(ac => ac.Company)
            //    .WithMany(c => c.ApplicationUserCompanies)
            //    .HasForeignKey(ac => ac.CompanyId);

            builder.Entity<Points>().HasKey(p => new { p.ApplicationUserId, p.CompanyId });
            builder.Entity<Company>().Property(b => b.PointsToEuroRatio).HasDefaultValue(0.001);
            builder.Entity<Company>().Property(b => b.EuroToPointsRatio).HasDefaultValue(0.2);

            // builder.Entity<Store>().HasKey(p => new { p.Id, p.CompanyId, p.Address });
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
        // public DbSet<ApplicationUserCompany> ApplicationUserCompanies { get; set; }
        public DbSet<PointsHistory> PointsHistory { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Sales> Sales { get; set; }

    }
}