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
            if (db.Products.Count() == 0)
            {
                ImportSpecifications("Data/Import/specifications.json", db);
                ImportBenchmarks("Data/Import/benchmarks.json", db);
                ImportProducts("Data/Import/products.json", db);
                ImportSources(db);
            }
        }

        private static void ImportBenchmarks(string path, ApplicationDbContext db)
        {
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                var imported = JsonConvert.DeserializeObject<List<Benchmark>>(File.ReadAllText(path));
                var fromDb = db.Benchmarks.ToList();

                foreach (var import in imported)
                {
                    var existing = fromDb.FirstOrDefault(b => b == import);
                    if (existing == null)
                    {
                        import.ID = 0;
                        db.Benchmarks.Add(import);
                        db.SaveChanges();
                    }
                    else if (existing != import)
                    {
                        existing.Name = import.Name;
                        existing.Type = import.Type;
                        db.SaveChanges();
                    }
                }

                foreach(var existing in fromDb)
                {
                    if(!imported.Any(b => b.ID == existing.ID))
                    {
                        db.Remove(existing);
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
                        import.ID = 0;
                        foreach (var s in import.Specs)
                        {
                            s.Id = 0;
                        }
                        db.Products.Add(import);
                        db.SaveChanges();
                        existing = import;
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

                    foreach (var existingSpec in existing.Specs)
                    {
                        if (!import.Specs.Any(b => b == existingSpec))
                        {
                            db.Remove(existingSpec);
                            db.SaveChanges();
                        }
                    }

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void ImportSpecifications(string path, ApplicationDbContext db)
        {
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                var imported = JsonConvert.DeserializeObject<List<Specification>>(File.ReadAllText(path));
                var fromDb = db.Specifications.ToList();

                foreach (var import in imported)
                {
                    var existing = fromDb.FirstOrDefault(b => b == import);
                    if (existing == null)
                    {
                        import.Id = 0;
                        db.Specifications.Add(import);
                        db.SaveChanges();
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
