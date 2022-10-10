using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBenchmark.Models
{
    public interface ISpecAttachable
    {
        public long Id { get; set; }
        public List<SpecificationEntry> Specs { get; set; }
    }
}
