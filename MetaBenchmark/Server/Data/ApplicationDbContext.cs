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
using System.IO;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MetaBenchmark.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Benchmark> Benchmarks { get; set; }
        public DbSet<BenchmarkEntry> BenchmarkEntries { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<SpecificationEntry> SpecificationEntries { get; set; }
        public DbSet<BenchmarkSource> BenchmarkSources { get; set; }
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
            base.OnConfiguring(optionsBuilder);
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

            builder.Entity<BenchmarkEntry>()
                .HasKey(e => e.Id);

            builder.Entity<BenchmarkEntry>()
                .HasOne(e => e.Product)
                .WithMany(p => p.BenchmarkEntries)
                .HasForeignKey(e => e.ProductId);

            builder.Entity<BenchmarkEntry>()
                .HasOne(e => e.Benchmark)
                .WithMany(b => b.Entries)
                .HasForeignKey(e => e.BenchmarkId);

            builder.Entity<BenchmarkEntry>()
                .HasOne(e => e.Source)
                .WithMany(s => s.BenchmarkEntries)
                .HasForeignKey(e => e.SourceId);

            //SeedBenchmarks("Data/Import/benchmarks.json", builder);
            //SeedSpecifications("Data/Import/specifications.json", builder);
            //SeedProducts("Data/Import/products.json", builder);
            //SeedSources(builder);

            base.OnModelCreating(builder);
        }

        private void SeedBenchmarks(string path, ModelBuilder builder)
        {
            if(!File.Exists(path))
            {
                return;
            }

            var items = JsonConvert.DeserializeObject<List<Benchmark>>(File.ReadAllText(path));
            builder.Entity<Benchmark>().HasData(items);
        }

        private void SeedSpecifications(string path, ModelBuilder builder)
        {
            if (!File.Exists(path))
            {
                return;
            }

            var items = JsonConvert.DeserializeObject<List<Specification>>(File.ReadAllText(path));
            builder.Entity<Specification>().HasData(items);
        }

        private void SeedProducts(string path, ModelBuilder builder)
        {
            if (!File.Exists(path))
            {
                return;
            }

            var items = JsonConvert.DeserializeObject<List<Product>>(File.ReadAllText(path));
            foreach(var item in items)
            {
                var specs = item.Specs;
                item.Specs = null;
                builder.Entity<Product>().HasData(item);
                builder.Entity<SpecificationEntry>().HasData(specs);
            }
        }

        private void SeedSources(ModelBuilder builder)
        {
            if (Directory.Exists("Data/Import/sources"))
            {
                foreach (var path in Directory.EnumerateFiles("Data/Import/sources"))
                {
                    var item = JsonConvert.DeserializeObject<BenchmarkSource>(File.ReadAllText(path));
                    var bench = item.BenchmarkEntries;
                    item.BenchmarkEntries = null;
                    builder.Entity<BenchmarkSource>().HasData(item);
                    builder.Entity<BenchmarkEntry>().HasData(bench);
                }
            }
        }
    }
}