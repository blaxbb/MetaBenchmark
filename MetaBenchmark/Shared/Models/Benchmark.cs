using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaBenchmark.Shared
{
    public class Benchmark
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public enum BenchmarkType
        {
            FPS,
            Compute,
            Timing
        }
        public BenchmarkType Type { get; set; }
        public List<BenchmarkEntry> Entries { get; set; }
    }
}