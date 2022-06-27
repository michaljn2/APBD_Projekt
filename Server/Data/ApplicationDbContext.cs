using BlazorApp1.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<UsersCompanies> UsersCompanies { get; set; }
        public DbSet<CompanyDailyTrades> CompanyDailyTrades { get; set; }
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Company>(e =>
            {
                e.HasKey(e => e.IdCompany);
                e.Property(e => e.Name);
                e.Property(e => e.City);
                e.Property(e => e.Logo);
                e.Property(e => e.Description);
                e.ToTable("Company");
            });

            builder.Entity<UsersCompanies>(e =>
            {
                e.HasKey(e => new { e.IdUser, e.IdCompany });
                e.HasOne(e => e.Company).WithMany(e => e.UsersCompanies).HasForeignKey(e => e.IdCompany).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(e => e.User).WithMany(e => e.UsersCompanies).HasForeignKey(e => e.IdUser).OnDelete(DeleteBehavior.Cascade);
                e.ToTable("Users_Companies");
            });

            builder.Entity<CompanyDailyTrades>(e =>
            {
                e.HasKey(e => new { e.IdCompany, e.DateTime });
                e.HasOne(e => e.Company).WithMany(e => e.CompanyDailyTrades).HasForeignKey(e => e.IdCompany).OnDelete(DeleteBehavior.Cascade);
                e.Property(e => e.Json);
                e.ToTable("Company_Daily_Trades");
            });
        }
    }
}
