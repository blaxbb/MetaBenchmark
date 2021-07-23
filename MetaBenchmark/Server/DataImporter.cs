using MetaBenchmark.Server.Data;
using MetaBenchmark.Shared;
using MetaBenchmark.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MetaBenchmark.Server
{
    public static class DataImporter
    {

        public static void Import(ApplicationDbContext db)
        {
            db.Database.ExecuteSqlRaw("DELETE FROM [Benchmarks]");
            db.Database.ExecuteSqlRaw("DELETE FROM [BenchmarkSources]");
            db.Database.ExecuteSqlRaw("DELETE FROM [Products]");
            db.Database.ExecuteSqlRaw("DELETE FROM [Specifications]");
            db.Database.ExecuteSqlRaw("DELETE FROM [SpecificationEntries]");
            db.Database.ExecuteSqlRaw("DELETE FROM [BenchmarkEntries]");

            db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ([Benchmarks], RESEED, 0)");
            db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ([BenchmarkSources], RESEED, 0)");
            db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ([Products], RESEED, 0)");
            db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ([Specifications], RESEED, 0)");
            db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ([SpecificationEntries], RESEED, 0)");
            db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ([BenchmarkEntries], RESEED, 0)");

            if (db.Products.Count() == 0)
            {
                ImportSpecifications(db);
                ImportBenchmarks(db);
                ImportProducts("Data/Import/products.json", db);
                ImportSources(db);
            }
        }

        private static void ImportBenchmarks(ApplicationDbContext db)
        {
            foreach (var file in Directory.EnumerateFiles("Data/Import/benchmarks"))
            {
                ImportBenchmark(file, db);
            }
        }

        private static void ImportBenchmark(string path, ApplicationDbContext db)
        {
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                var typeName = Path.GetFileNameWithoutExtension(path);
                var type = (Benchmark.BenchmarkType)Enum.Parse(typeof(Benchmark.BenchmarkType), typeName);
                var imported = JsonConvert.DeserializeObject<List<Benchmark>>(File.ReadAllText(path));
                var fromDb = db.Benchmarks.ToList();

                foreach (var import in imported)
                {
                    import.Type = type;

                    var existing = fromDb.FirstOrDefault(b => b == import);
                    if (existing == null)
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            var entityType = db.Model.FindEntityType(typeof(Benchmark));
                            db.Benchmarks.Add(import);
                            db.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} ON;");
                            db.SaveChanges();
                            db.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} OFF");
                            transaction.Commit();
                        }
                    }
                    else if (existing != import)
                    {
                        existing.Name = import.Name;
                        existing.Type = import.Type;
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void ImportProducts(string path, ApplicationDbContext db)
        {
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                var imported = JsonConvert.DeserializeObject<List<Product>>(File.ReadAllText(path));
                var fromDb = db.Products.Include(p => p.Specs).ToList();

                foreach (var import in imported)
                {
                    var existing = fromDb.FirstOrDefault(b => b == import);
                    if (existing == null)
                    {
                        foreach (var s in import.Specs)
                        {
                            s.Id = 0;
                            s.ProductId = import.ID;
                        }

                        using (var transaction = db.Database.BeginTransaction())
                        {
                            var entityType = db.Model.FindEntityType(typeof(Product));
                            db.Products.Add(import);
                            db.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} ON;");
                            db.SaveChanges();
                            db.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} OFF");
                            transaction.Commit();
                        }
                    }
                    else
                    {
                        if (existing != import)
                        {
                            existing.Name = import.Name;
                            existing.Type = import.Type;
                            db.SaveChanges();
                        }

                        foreach (var spec in import.Specs)
                        {
                            var existingSpec = existing.Specs?.FirstOrDefault(b => b == spec);
                            if (existingSpec == null)
                            {
                                spec.Id = 0;
                                db.SpecificationEntries.Add(spec);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void ImportSpecifications(ApplicationDbContext db)
        {
            foreach (var file in Directory.EnumerateFiles("Data/Import/specifications"))
            {
                ImportSpecification(file, db);
            }
        }

        private static void ImportSpecification(string path, ApplicationDbContext db)
        {
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                var name = Path.GetFileNameWithoutExtension(path);

                var imported = JsonConvert.DeserializeObject<List<Specification>>(File.ReadAllText(path));
                var fromDb = db.Specifications.ToList();

                foreach (var import in imported)
                {
                    import.Name = name;
                    var existing = fromDb.FirstOrDefault(b => b == import);
                    if (existing == null)
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            var entityType = db.Model.FindEntityType(typeof(Specification));
                            db.Specifications.Add(import);
                            db.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} ON;");
                            db.SaveChanges();
                            db.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} OFF");
                            transaction.Commit();
                        }
                    }
                    else if (existing != import)
                    {
                        existing.Name = import.Name;
                        existing.Value = import.Value;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void ImportSources(ApplicationDbContext db)
        {
            foreach(var file in Directory.EnumerateFiles("Data/Import/sources"))
            {
                ImportSource(file, db);
            }
        }

        private static void ImportSource(string path, ApplicationDbContext db)
        {
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                var import = JsonConvert.DeserializeObject<BenchmarkSource>(File.ReadAllText(path));
                var fromDb = db.BenchmarkSources.Include(b => b.BenchmarkEntries).ToList();

                var existing = fromDb.FirstOrDefault(b => b == import);
                if (existing == null)
                {
                    import.Id = 0;
                    foreach (var entry in import.BenchmarkEntries)
                    {
                        entry.Id = 0;
                    }

                    db.BenchmarkSources.Add(import);
                    db.SaveChanges();
                    existing = import;
                }
                else
                {
                    if (existing != import)
                    {
                        existing.Name = import.Name;
                        existing.LogoUrl = import.LogoUrl;
                        existing.Url = import.Url;
                        db.SaveChanges();
                    }

                    foreach (var bench in import.BenchmarkEntries)
                    {
                        var existingBench = existing.BenchmarkEntries?.FirstOrDefault(b => b == bench);
                        if (existingBench == null)
                        {
                            bench.Id = 0;
                            db.BenchmarkEntries.Add(bench);
                            db.SaveChanges();
                        }
                    }
                }

                foreach (var existingBench in existing.BenchmarkEntries)
                {
                    if (!import.BenchmarkEntries.Any(b => b == existingBench))
                    {
                        db.Remove(existingBench);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
