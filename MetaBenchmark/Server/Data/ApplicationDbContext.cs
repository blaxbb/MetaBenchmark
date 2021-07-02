using IdentityServer4.EntityFramework.Options;
using MetaBenchmark.Shared;
using MetaBenchmark.Server.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaBenchmark.Shared.Models;

namespace MetaBenchmark.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Benchmark> Benchmarks { get; set; }
        public DbSet<BenchmarkEntry> BenchmarkEntries { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<SpecificationEntry> SpecificationEntries { get; set; }
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SpecificationEntry>()
                .HasKey(e => e.Id);

            builder.Entity<SpecificationEntry>()
                .HasOne(e => e.Spec)
                .WithMany(p => p.Products)
                .HasForeignKey(e => e.SpecId);

            builder.Entity<SpecificationEntry>()
                .HasOne(e => e.Product)
                .WithMany(p => p.Specs)
                .HasForeignKey(e => e.ProductId);


            builder.Entity<Specification>()
                .HasIndex(s => s.Name);

            base.OnModelCreating(builder);
        }
    }
}