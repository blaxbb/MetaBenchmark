using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBenchmark.Models
{
    public class SpecificationEntry
    {
        public long Id { get; set; }
        public Specification Spec { get; set; }
        public long SpecId { get; set; }

        public ISpecAttachable Item { get; set; }
        public long ItemId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SpecificationEntry entry &&
                   SpecId == entry.SpecId &&
                   ItemId == entry.ItemId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpecId, ItemId);
        }

        public static bool operator ==(SpecificationEntry left, SpecificationEntry right)
        {
            return EqualityComparer<SpecificationEntry>.Default.Equals(left, right);
        }

        public static bool operator !=(SpecificationEntry left, SpecificationEntry right)
        {
            return !(left == right);
        }

    }
}
