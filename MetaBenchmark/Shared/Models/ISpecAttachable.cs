using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBenchmark.Shared.Models
{
    public interface ISpecAttachable
    {
        public long Id { get; set; }
        public ICollection<SpecificationEntry> Specs { get; set; }
    }
}
