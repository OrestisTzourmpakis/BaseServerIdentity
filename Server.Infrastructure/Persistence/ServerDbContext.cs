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
            builder.Entity<Company>().HasOne(p => p.Category).WithMany(p => p.Companies).HasForeignKey(l => l.CategoryId).OnDelete(DeleteBehavior.SetNull);
            // builder.Entity<Store>().HasKey(p => new { p.Id, p.CompanyId, p.Address });
            base.OnModelCreating(builder);
        }
        // when we add a user in the table!!
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<ApplicationUser>())
            {
                // has the email, username here!
                // entry.Entity.UserName = entry.Entity.UserName + "aaa";
                // entry.Entity.NormalizedUserName = entry.Entity.NormalizedUserName + "AAA";

            }

            foreach (var entry in ChangeTracker.Entries<PointsHistory>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.TransactionDate = DateTime.Now;
                }
            }

            foreach (var entry in ChangeTracker.Entries<ApplicationUser>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.DateJoined = DateTime.Now;
                }
            }
            foreach (var entry in ChangeTracker.Entries<Points>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.UserJoined = DateTime.Now;
                }
            }

            foreach (var entry in ChangeTracker.Entries<Points>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.UserJoined = DateTime.Now;
                }
            }

            foreach (var entry in ChangeTracker.Entries<Store>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Created = DateTime.Now;
                }
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