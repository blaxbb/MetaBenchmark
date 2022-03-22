using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaBenchmark
{
    public static class ListExtensions
    {
        [ThreadStatic]
        private static Random rand = new System.Random();
        public static IEnumerable<T> Random<T>(this List<T> list)
        {
            return list.OrderBy(i => rand.Next());
        }

        public static T RandomItem<T>(this List<T> list)
        {
            if(list == default || list.Count == 0)
            {
                return default(T);
            }

            return list.RandomItem(rand);
        }

        public static T RandomItem<T>(this List<T> list, Random random)
        {
            if (list == default || random == null || list.Count == 0)
            {
                return default(T);
            }

            var index = random.Next(list.Count);
            return list[index];
        }
    }
}
