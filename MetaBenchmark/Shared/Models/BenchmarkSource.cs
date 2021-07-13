using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBenchmark.Shared.Models
{
    public class BenchmarkSource
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string LogoUrl { get; set; }
        public ICollection<BenchmarkEntry> BenchmarkEntries { get; set; }

        public override bool Equals(object obj)
        {
            return obj is BenchmarkSource source &&
                   Name == source.Name &&
                   Url == source.Url &&
                   LogoUrl == source.LogoUrl;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Url, LogoUrl);
        }
        public static bool operator ==(BenchmarkSource left, BenchmarkSource right)
        {
            return EqualityComparer<BenchmarkSource>.Default.Equals(left, right);
        }

        public static bool operator !=(BenchmarkSource left, BenchmarkSource right)
        {
            return !(left == right);
        }
    }
}
