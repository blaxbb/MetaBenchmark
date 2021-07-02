using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MetaBenchmark.Shared
{
    public class BenchmarkEntry
    {
        public long Id { get; set; }

        public Benchmark Benchmark { get; set; }
        public long BenchmarkId { get; set; }
        public Product Product { get; set; }
        public long ProductId { get; set; }
        public double Value { get; set; }
    }
}
