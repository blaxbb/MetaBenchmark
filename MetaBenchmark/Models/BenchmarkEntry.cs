using MetaBenchmark.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MetaBenchmark
{
    public class BenchmarkEntry : ISpecAttachable
    {
        public long Id { get; set; }

        public Benchmark Benchmark { get; set; }
        public long BenchmarkId { get; set; }
        public Product Product { get; set; }
        public long ProductId { get; set; }
        public double Value { get; set; }
        public string Url { get; set; }

        public BenchmarkSource Source { get; set; }
        public long SourceId { get; set; }
        public ICollection<SpecificationEntry> Specs { get; set; }

        public override bool Equals(object obj)
        {
            return obj is BenchmarkEntry entry &&
                   BenchmarkId == entry.BenchmarkId &&
                   ProductId == entry.ProductId &&
                   Value == entry.Value &&
                   Url == entry.Url &&
                   SourceId == entry.SourceId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BenchmarkId, ProductId, Value, SourceId);
        }

        public static bool operator ==(BenchmarkEntry left, BenchmarkEntry right)
        {
            return EqualityComparer<BenchmarkEntry>.Default.Equals(left, right);
        }

        public static bool operator !=(BenchmarkEntry left, BenchmarkEntry right)
        {
            return !(left == right);
        }

        public string ValueLabel() {
            switch (Benchmark?.Type ?? Benchmark.BenchmarkType.FPS)
            {
                case Benchmark.BenchmarkType.FPS:
                case Benchmark.BenchmarkType.FPS_1080P:
                case Benchmark.BenchmarkType.FPS_1440P:
                case Benchmark.BenchmarkType.FPS_4K:
                    return $"{Value:0.0} FPS";
                case Benchmark.BenchmarkType.Compute:
                    return $"{Value:0.0} Pts";
                case Benchmark.BenchmarkType.Timing:
                    return Value switch
                    {
                        < 1000 => $"{Value} ms",
                        < 1000 * 60 => $"{Value / (1000):.0} sec",
                        _ => $"{Value / (1000 * 60):.00} min",

                    };
                default:
                    return Value.ToString();
            }
        }


    }
}
