using MetaBenchmark;
using MetaBenchmark.Models;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace MetaBenchmark.Services
{
    public class DataService
    {
        Dictionary<long, Product> Products;
        Dictionary<long, Benchmark> Benchmarks;
        List<BenchmarkSource> Sources;

        Dictionary<string, List<Specification>> BenchmarkSpecifications;
        Dictionary<long, Specification> AllBenchmarkSpecifications;
        Dictionary<string, List<Specification>> BenchmarkEntrySpecifications;
        Dictionary<long, Specification> AllBenchmarkEntrySpecifications;
        Dictionary<string, List<Specification>> ProductSpecifications;
        Dictionary<long, Specification> AllProductSpecifications;

        public DataService()
        {
            Products = new ();
            Benchmarks = new();
            Sources = new ();
            BenchmarkSpecifications = new();
            BenchmarkEntrySpecifications = new();
            ProductSpecifications = new();
        }

        #region Controller
        public List<Product> AllProducts()
        {
            return Products.Values.ToList();
        }

        public Product? Product(long id)
        {
            return Products.ContainsKey(id) ? Products[id] : default;
        }

        public List<Benchmark> AllBenchmarks()
        {
            return Benchmarks.Values.ToList();
        }

        public Benchmark? Benchmark(long id)
        {
            return Benchmarks.ContainsKey(id) ? Benchmarks[id] : default;
        }

        public List<Specification> GetAllProductSpecifications()
        {
            return AllProductSpecifications.Values.ToList();
        }

        #endregion

        #region Initialize

        public async Task Initialize()
        {
            await InitializeSpecifications();
            await InitializeProducts();
            await InitializeBenchmarks();
            await InitializeSources();
        }

        private async Task InitializeProducts()
        {
            var products = await LoadJson<List<Product>>("data/products.json");

            if (products is null)
            {
                Console.WriteLine($"Error loading products!");
                return;
            }

            AttachSpecs(products, AllProductSpecifications);

            Products = products.ToDictionary(p => p.Id, p => p);
        }

        private async Task InitializeBenchmarks()
        {
            Benchmarks = new Dictionary<long, Benchmark>();
            foreach (var benchType in Enum.GetValues(typeof(Benchmark.BenchmarkType)).Cast<Benchmark.BenchmarkType>())
            {
                try
                {
                    var benches = await LoadJson<List<Benchmark>>($"data/benchmarks/{benchType.ToString()}.json");

                    if (benches is null)
                    {
                        Console.WriteLine($"Error loading benchmarks!");
                        continue;
                    }

                    AttachSpecs(benches, AllBenchmarkSpecifications);

                    foreach(var b in benches)
                    {
                        b.Type = benchType;

                        Benchmarks.Add(b.Id, b);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Loading data/benchmarks/{benchType}.json failed!");
                }
            }
        }

        private async Task InitializeSources()
        {
            var allSources = await LoadJson<List<string>>("data/sources/index.json");
            foreach (var sourceName in allSources)
            {
                var source = await LoadJson<BenchmarkSource>($"data/sources/{sourceName}.json");

                if (source is null)
                {
                    Console.WriteLine($"Error loading source {sourceName}");
                    continue;
                }

                AttachSpecs(source.BenchmarkEntries, AllBenchmarkEntrySpecifications);

                foreach (var entry in source.BenchmarkEntries)
                {
                    entry.Source = source;
                    entry.SourceId = source.Id;

                    if (Benchmarks.ContainsKey(entry.BenchmarkId))
                    {
                        var bench = Benchmarks[entry.BenchmarkId];
                        entry.Benchmark = bench;

                        bench.Entries.Add(entry);
                    }

                    if (Products.ContainsKey(entry.ProductId))
                    {
                        var product = Products[entry.ProductId];
                        entry.Product = product;
                        product.BenchmarkEntries.Add(entry);
                    }
                }

                Sources.Add(source);
            }
        }

        async Task InitializeSpecifications()
        {
            BenchmarkSpecifications = await InitializeSpecifications(Specification.ItemType.Benchmark) ?? new();
            BenchmarkEntrySpecifications = await InitializeSpecifications(Specification.ItemType.BenchmarkEntry) ?? new();
            ProductSpecifications = await InitializeSpecifications(Specification.ItemType.Product) ?? new();

            AllBenchmarkSpecifications = BenchmarkSpecifications.SelectMany(kvp => kvp.Value).ToDictionary(s => s.Id, s => s);
            AllBenchmarkEntrySpecifications = BenchmarkEntrySpecifications.SelectMany(kvp => kvp.Value).ToDictionary(s => s.Id, s => s);
            AllProductSpecifications = ProductSpecifications.SelectMany(kvp => kvp.Value).ToDictionary(s => s.Id, s => s);
        }

        async Task<Dictionary<string, List<Specification>>?> InitializeSpecifications(Specification.ItemType type)
        {
            var index = await LoadJson<List<string>>($"data/specifications/{type}/index.json");
            if(index is null)
            {
                Console.WriteLine($"Error loading specification index for {type}");
                return default;
            }

            var allSpecs = new Dictionary<string, List<Specification>>();

            foreach(var file in index)
            {
                var specs = await LoadJson<List<Specification>>($"data/specifications/{type}/{file}.json");
                if(specs is null)
                {
                    Console.WriteLine("Error loading specification file {cat} {file}");
                    continue;
                }

                specs.ForEach(s => {
                    s.Name = file;
                    s.Type = type;
                });

                allSpecs.Add(file, specs);
            }

            return allSpecs;
        }

        void AttachSpecs(IEnumerable<ISpecAttachable> objs, Dictionary<long, Specification> specs)
        {
            foreach (var obj in objs)
            {
                foreach (var spec in obj.Specs)
                {
                    if (specs.ContainsKey(spec.SpecId))
                    {
                        spec.Item = obj;
                        spec.ItemId = obj.Id;
                        spec.Spec = specs[spec.SpecId];
                    }
                }
            }
        }

        async Task<T?> LoadJson<T>(string path) where T : class
        {
            try
            {
                var json = await File.ReadAllTextAsync(path);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return default(T);
            }
        }
        #endregion
    }
}