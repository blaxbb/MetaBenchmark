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
    }
}