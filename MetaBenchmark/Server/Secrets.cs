using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaBenchmark.Server
{
    public class Secrets
    {
        public const string SecretsName = "Secrets";
        public string DbPassword { get; set; }
        public string AdminEmail { get; set; }
    }
}
