﻿using MetaBenchmark.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaBenchmark
{
    public class Product : ISpecAttachable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public enum ProductType
        {
            Unknown,
            CPU,
            GPU
        }
        public ProductType Type { get; set; }
        public ICollection<BenchmarkEntry> BenchmarkEntries { get; set; }
        public ICollection<SpecificationEntry> Specs { get; set; }

        [JsonIgnore]
        public string Brand => Specs?.FirstOrDefault(s => s?.Spec?.Name == "brand")?.Spec.Value;

        public Product()
        {
            BenchmarkEntries = new List<BenchmarkEntry>();
            Specs = new List<SpecificationEntry>();
        }

        public override bool Equals(object obj)
        {
            return obj is Product product &&
                   Name == product.Name &&
                   Type == product.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type);
        }

        public static bool operator ==(Product left, Product right)
        {
            return EqualityComparer<Product>.Default.Equals(left, right);
        }

        public static bool operator !=(Product left, Product right)
        {
            return !(left == right);
        }
    }
}