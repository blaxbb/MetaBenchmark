﻿using System;
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

        public override bool Equals(object obj)
        {
            return obj is SpecificationEntry entry &&
                   SpecId == entry.SpecId &&
                   ProductId == entry.ProductId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpecId, ProductId);
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
