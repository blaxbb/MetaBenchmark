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
            switch (name)
            {
                case DataCache.NAME_ALL:
                case DataCache.NAME_BENCHMARKS:
                case DataCache.NAME_PRODUCTS:
                case DataCache.NAME_SPECIFICATIONS:
                case DataCache.NAME_SOURCES:
                    await js.InvokeVoidAsync("DBSetAll", name, value);
                    break;
                case "mb.settings":
                    await js.InvokeVoidAsync("DBSetKeyVal", "Settings", "Settings", value);
                    break;
            }

            var ret = new StorageEntry<T>(name, userModified, value);
            return ret;
        }

        public static async Task Clear(IJSRuntime js, string name)
        {
            switch (name)
            {
                case DataCache.NAME_ALL:
                case DataCache.NAME_BENCHMARKS:
                case DataCache.NAME_PRODUCTS:
                case DataCache.NAME_SPECIFICATIONS:
                case DataCache.NAME_SOURCES:
                case "Settings":
                    await js.InvokeVoidAsync("dbclear", name);
                    break;
            }
        }

        public static async Task<StorageEntry<T>?> GetValue(IJSRuntime js, string name)
        {
            switch (name)
            {
                case DataCache.NAME_ALL:
                case DataCache.NAME_BENCHMARKS:
                case DataCache.NAME_PRODUCTS:
                case DataCache.NAME_SPECIFICATIONS:
                case DataCache.NAME_SOURCES:
                    var val = await js.InvokeAsync<T>("dbgetall", name);
                    if(val is IList l && l.Count == 0)
                    {
                        return default;
                    }
                    return new StorageEntry<T>(name, false, val);
                case "mb.settings":
                    var settings = await js.InvokeAsync<T>("dbget", "Settings", "Settings");
                    if(settings != null)
                    {
                        return new StorageEntry<T>(name, false, settings);
                    }
                    return default;
            }

            return default;
        }
    }
}
