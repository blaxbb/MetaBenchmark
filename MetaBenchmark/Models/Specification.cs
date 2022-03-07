using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBenchmark.Models
{
    public class Specification
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public ICollection<SpecificationEntry> Products { get; set; }

        public ItemType Type { get; set; }
        public enum ItemType
        {
            Product,
            Benchmark
        }

        public override bool Equals(object obj)
        {
            return obj is Specification specification &&
                   Name == specification.Name &&
                   Value == specification.Value &&
                   Type == specification.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Value, Type);
        }

        public static bool operator ==(Specification left, Specification right)
        {
            return EqualityComparer<Specification>.Default.Equals(left, right);
        }

        public static bool operator !=(Specification left, Specification right)
        {
            return !(left == right);
        }
    }
}