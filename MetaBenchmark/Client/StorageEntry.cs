using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaBenchmark.Client
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
            var ret = new StorageEntry<T>(name, userModified, value);
            await ret.SetValue(js);
            return ret;
        }

        public static async Task Clear(IJSRuntime js, string name)
        {
            await js.InvokeVoidAsync("SetStorage", name, null);
        }

        public static async Task<StorageEntry<T>?> GetValue(IJSRuntime js, string name)
        {
            var json = await js.InvokeAsync<string>("GetStorage", name);
            if(string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<StorageEntry<T>>(json);
        }
    }
}
