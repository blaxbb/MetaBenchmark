using MetaBenchmark.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaBenchmark.Models
{
    public class Product : ISpecAttachable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public enum ProductType
        {
            Unknown,
            CPU,
            GPU
        }
        public ProductType Type { get; set; }
        public List<BenchmarkEntry> BenchmarkEntries { get; set; }
        public List<SpecificationEntry> Specs { get; set; }

        [JsonIgnore]
        public string Brand => Specs?.FirstOrDefault(s => s?.Spec?.Name == "brand")?.Spec.Value;

        public Product()
        {
            BenchmarkEntries = new List<BenchmarkEntry>();
            Specs = new List<SpecificationEntry>();
        }

        public override bool Equals(object obj)
        {
            return obj is Product product &&
                   Name == product.Name &&
                   Type == product.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type);
        }

        public static bool operator ==(Product left, Product right)
        {
            return EqualityComparer<Product>.Default.Equals(left, right);
        }

        public static bool operator !=(Product left, Product right)
        {
            return !(left == right);
        }

        public Product ViewModel() => new Product()
        {
            Name = Name,
            BenchmarkEntries = BenchmarkEntries.Select(b => new BenchmarkEntry()
            {
                Benchmark = b.Benchmark.StripEntries(),
                Value = b.Value,
                Url = b.Url,
                Specs = b.Specs.Select(s => s.StripParent()).ToList(),
                Source = b.Source.StripEntries()
            }).ToList(),
            Id = Id,
            Specs = Specs.Select(s => s.StripParent()).ToList(),
            Type = Type,
        };

        public Product StripEntries()
        {
            return new Product()
            {
                Name = Name,
                Id = Id,
                Specs = Specs.Select(s => s.StripParent()).ToList(),
                Type = Type,
                BenchmarkEntries = default
            };
        }
    }
}