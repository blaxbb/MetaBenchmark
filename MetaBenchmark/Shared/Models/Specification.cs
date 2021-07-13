using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBenchmark.Shared.Models
{
    public class Specification
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public ICollection<SpecificationEntry> Products { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Specification specification &&
                   Name == specification.Name &&
                   Value == specification.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Value);
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