using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            Timing,
            FPS_4K,
            FPS_1440P,
            FPS_1080P
        }
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

    public static class BenchmarkExtensions
    {
        public static string DisplayName(this Benchmark.BenchmarkType type) => type switch
        {
            Benchmark.BenchmarkType.FPS => "FPS",
            Benchmark.BenchmarkType.Compute => "Compute",
            Benchmark.BenchmarkType.Timing => "Timing",
            Benchmark.BenchmarkType.FPS_4K => "FPS 4K",
            Benchmark.BenchmarkType.FPS_1440P => "FPS 1440p",
            Benchmark.BenchmarkType.FPS_1080P => "FPS 1080p",
            _ => type.ToString(),
        };
    }
}