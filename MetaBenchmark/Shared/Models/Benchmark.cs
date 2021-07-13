using Newtonsoft.Json;
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
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public BenchmarkType Type { get; set; }
        public List<BenchmarkEntry> Entries { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Benchmark benchmark &&
                   Name == benchmark.Name &&
                   Type == benchmark.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type);
        }

        public static bool operator ==(Benchmark left, Benchmark right)
        {
            return EqualityComparer<Benchmark>.Default.Equals(left, right);
        }

        public static bool operator !=(Benchmark left, Benchmark right)
        {
            return !(left == right);
        }
    }
}