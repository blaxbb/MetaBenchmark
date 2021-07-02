using MetaBenchmark.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaBenchmark.Shared
{
    public class Product
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public enum ProductType
        {
            Unknown,
            CPU,
            GPU
        }
        public ProductType Type { get; set; }
        public List<BenchmarkEntry> BenchmarkEntries { get; set; }
        public ICollection<SpecificationEntry> Specs { get; set; }
    }
}