using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBenchmark.Shared.Models
{
    public class SpecificationEntry
    {
        public long Id { get; set; }
        public Specification Spec { get; set; }
        public long SpecId { get; set; }

        public Product Product { get; set; }
        public long ProductId { get; set; }
    }
}
