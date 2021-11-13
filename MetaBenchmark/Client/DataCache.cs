using MetaBenchmark.Client;
using MetaBenchmark.Shared;
using MetaBenchmark.Shared.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MetaBenchmark.Client
{
    public class DataCache
    {

        bool CACHE_ENABLED = true;
        bool Initialized = false;
        IJSRuntime js;
        HttpClient client;

        const string NAME_ALL = "mb.AllProducts";
        const string NAME_PRODUCTS = "mb.Products";
        const string NAME_BENCHMARKS = "mb.Benchmarks";
        const string NAME_SPECIFICATIONS = "mb.Specifications";
        const string NAME_SOURCES = "mb.Sources";

        public DataCache(IJSRuntime js, HttpClient client)
        {
            Console.WriteLine($"INIT CACHE {(js == null ? "NULL" : "HAVE JS")}");
            this.js = js;
            this.client = client;
        }

        async Task Initialize()
        {
            if(Initialized)
            {
                return;
            }
            Initialized = true;
            var settings = await Settings.Load(js);
            CACHE_ENABLED = settings.CacheEnabled;
        }

        public async Task<StorageEntry<List<Product>>> All()
        {
            return await Get(NAME_ALL, GetAllProducts);
        }

        public async Task<StorageEntry<List<Specification>>> Specifications()
        {
            return await Get(NAME_SPECIFICATIONS, FetchSpecifications);
        }

        public async Task<StorageEntry<List<Benchmark>>> Benchmarks()
        {
            var specs = await Specifications();
            if(specs != null)
            {
                return await Benchmarks(specs.Value);
            }
            return default;
        }

        public async Task<StorageEntry<List<Benchmark>>> Benchmarks(List<Specification> specs)
        {
            return await Get(NAME_BENCHMARKS, async () => await FetchBenchmarks(specs));
        }

        public async Task<StorageEntry<List<BenchmarkSource>>> Sources()
        {
            return await Get(NAME_SOURCES, FetchSources);
        }

        async Task<StorageEntry<T>> Get<T>(string name, Func<Task<T>> fetchTask)
        {
            return await Get<T>(name, async () => (await fetchTask(), false));
        }

        async Task<StorageEntry<T>> Get<T>(string name, Func<Task<(T Value, bool userModified)>> fetchTask)
        {
            await Initialize();

            var existing = await StorageEntry<T>.GetValue(js, name);
            if (existing != null && (existing.UserModified || (DateTime.Now - existing.Created).Minutes < 15 && CACHE_ENABLED))
            {
                Console.WriteLine("CACHED");
                return existing;
            }
            else
            {
                Console.WriteLine("CACHE MISS");
                var fetched = await fetchTask();
                return await StorageEntry<T>.SetValue(js, name, fetched.userModified, fetched.Value);
            }
        }

        Task<T> Fetch<T>(string path)
        {
            return client.GetFromJsonAsync<T>(path);
        }

        async Task<(List<Product>, bool)> GetAllProducts()
        {
            var specs = await Specifications();
            var benchmarks = await Benchmarks(specs.Value);
            var sources = await Sources();

            var products = await Get(NAME_PRODUCTS, () => FetchProducts(specs.Value));
            foreach (var source in sources.Value)
            {
                foreach (var entry in source.BenchmarkEntries)
                {
                    entry.Source = new BenchmarkSource()
                    {
                        Id = source.Id,
                        LogoUrl = source.LogoUrl,
                        Name = source.Name,
                        Url = source.Url
                    };
                    entry.SourceId = entry.Source.Id;

                    var product = products.Value.FirstOrDefault(p => p.Id == entry.ProductId);
                    var benchmark = benchmarks.Value.FirstOrDefault(b => b.Id == entry.BenchmarkId);
                    entry.Benchmark = benchmark;
                    entry.BenchmarkId = benchmark.Id;
                    entry.ProductId = product.Id;
                    product.BenchmarkEntries.Add(entry);
                }
            }

            return (products.Value, specs.UserModified || benchmarks.UserModified || sources.UserModified || products.UserModified);
        }

        async Task<List<Specification>> FetchSpecifications()
        {
            var specs = new List<Specification>();

            foreach(var type in Enum.GetValues(typeof(Specification.ItemType)))
            {
                var itemType = (Specification.ItemType)type;

                List<string> specIndex;
                try
                {
                    specIndex = await Fetch<List<string>>($"data/specifications/{itemType}/index.json");
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Loading data/specifications/{itemType}/index.json failed!");
                    continue;
                }

                foreach (var specName in specIndex)
                {
                    try
                    {
                        var url = $"data/specifications/{itemType}/{Uri.EscapeDataString(specName)}.json";
                        var specValues = await Fetch<List<Specification>>(url);
                        specValues.ForEach(s => {
                            s.Name = specName;
                            s.Type = itemType;
                        });
                        specs.AddRange(specValues);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Loading data/specifications/{itemType}/{specName}.json failed!");
                    }
                }
            }

            return specs;
        }

        async Task<List<Benchmark>> FetchBenchmarks(List<Specification> specs)
        {
            var benchmarks = new List<Benchmark>();
            foreach (var benchType in Enum.GetValues(typeof(Benchmark.BenchmarkType)).Cast<Benchmark.BenchmarkType>())
            {
                try
                {
                    var url = $"data/benchmarks/{Uri.EscapeDataString(benchType.ToString())}.json";
                    var benchEntries = await Fetch<List<Benchmark>>(url);
                    benchEntries.ForEach(b => {
                        b.Type = benchType;
                        b.Specs.ToList().ForEach(entry => entry.Spec = specs.FirstOrDefault(s => s.Id == entry.SpecId));
                    });
                    benchmarks.AddRange(benchEntries);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Loading data/benchmarks/{benchType}.json failed!");
                }
            }

            return benchmarks;
        }

        async Task<List<BenchmarkSource>> FetchSources()
        {
            var sources = new List<BenchmarkSource>();
            var sourceIndex = await Fetch<List<string>>("data/sources/index.json");
            foreach (var sourceName in sourceIndex)
            {
                try
                {
                    var url = $"data/sources/{Uri.EscapeDataString(sourceName)}.json";
                    var source = await Fetch<BenchmarkSource>(url);
                    sources.Add(source);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Loading data/sources/{sourceName}.json failed!");
                    Console.WriteLine(e.Message);
                }
            }

            return sources;
        }

        async Task<List<Product>> FetchProducts(List<Specification> specs)
        {
            var products = await Fetch<List<Product>>("data/products.json");
            foreach (var product in products)
            {
                product.BenchmarkEntries = new List<BenchmarkEntry>();
                foreach (var spec in product.Specs)
                {
                    var s = specs.FirstOrDefault(s => s.Type == Specification.ItemType.Product && s.Id == spec.SpecId);
                    spec.Spec = s;
                }
            }

            return products;
        }

        async Task<StorageEntry<T>> SetModified<T>(string name, T data)
        {
            var ret = await StorageEntry<T>.SetValue(js, name, true, data);

            await StorageEntry<List<Product>>.Clear(js, NAME_ALL);

            return ret;
        }

        async Task<StorageEntry<T>> SetFromSource<T>(string name, T data)
        {
            return await StorageEntry<T>.SetValue(js, name, true, data);
        }

        public async Task<StorageEntry<List<Product>>> SetProducts(List<Product> data)
        {
            return await SetModified(NAME_PRODUCTS, data.Select(p => new Product()
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.Type,
                Specs = p.Specs
            }).ToList());
        }

        public async Task<StorageEntry<List<Specification>>> SetSpecifications(List<Specification> data)
        {
            return await SetModified(NAME_SPECIFICATIONS, data);
        }

        public async Task<StorageEntry<List<Benchmark>>> SetBenchmarks(List<Benchmark> data)
        {
            return await SetModified(NAME_BENCHMARKS, data);
        }

        public async Task<StorageEntry<List<BenchmarkSource>>> SetSources(List<BenchmarkSource> data)
        {
            return await SetModified(NAME_SOURCES, data);
        }

        public async Task Clear()
        {
            await js.InvokeVoidAsync("ClearStorage");
        }
    }
}
