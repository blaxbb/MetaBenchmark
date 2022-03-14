using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaBenchmark
{
    public class StorageEntry<T>
    {
        public string Name { get; set; }
        public bool UserModified { get; set; }
        public DateTime Created { get; set; }
        public T Value { get; set; }

        public StorageEntry(string name, bool userModified, T value)
        {
            Name = name;
            UserModified = userModified;
            Value = value;
            Created = DateTime.Now;
        }

        public async Task SetValue(IJSRuntime js)
        {
            await js.InvokeVoidAsync("SetStorage", Name, JsonConvert.SerializeObject(this));
        }

        public static async Task<StorageEntry<T>?> SetValue(IJSRuntime js, string name, bool userModified, T value)
        {
            if (typeof(T).IsAssignableTo(typeof(IList)))
            {
                await js.InvokeVoidAsync("DBSetAll", name, value);
            }
            else
            {
                await js.InvokeVoidAsync("DBSetKeyVal", name, name, value);
            }

            var ret = new StorageEntry<T>(name, userModified, value);
            return ret;
        }

        public static async Task Clear(IJSRuntime js, string name)
        {
            await js.InvokeVoidAsync("DBClear", name);
        }

        public static async Task<StorageEntry<T>?> GetValue(IJSRuntime js, string name)
        {
            T val;
            if(typeof(T).IsAssignableTo(typeof(IList)))
            {
                val = await js.InvokeAsync<T>("DBGetAll", name);
                if (val is IList l && l.Count == 0)
                {
                    return default;
                }
            }
            else
            {
                val = await js.InvokeAsync<T>("DBGet", name, name);
            }

            if (val == null)
            {
                return default;
            }

            return new StorageEntry<T>(name, false, val);
        }
    }
}
