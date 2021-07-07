using MetaBenchmark.Shared.Models;
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

        public BenchmarkSource Source { get; set; }
        public long SourceId { get; set; }

        public string ValueLabel() {
            switch (Benchmark?.Type ?? Benchmark.BenchmarkType.FPS)
            {
                case Benchmark.BenchmarkType.FPS:
                    return $"{Value:.} FPS";
                case Benchmark.BenchmarkType.Compute:
                    return $"{Value:.} Pts";
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
