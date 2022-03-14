﻿using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaBenchmark
{
    public class Settings
    {
        public bool CacheEnabled { get; set; } = true;
        public bool EditModeEnabled { get; set; } = false;

        public static async Task<Settings> Load(IJSRuntime js)
        {
            var settingsEntry = await StorageEntry<Settings>.GetValue(js, "Settings");
            var settings = settingsEntry?.Value;
            if (settings == null)
            {
                settings = new Settings();
            }

            return settings;
        }

        public async Task Save(IJSRuntime js)
        {
            await StorageEntry<Settings>.SetValue(js, "Settings", false, this);
        }
    }
}
