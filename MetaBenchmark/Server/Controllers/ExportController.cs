﻿using MetaBenchmark.Server.Data;
using MetaBenchmark.Server.Zip;
using MetaBenchmark.Shared;
using MetaBenchmark.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MetaBenchmark.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ExportController : Controller
    {
        ApplicationDbContext db;
        public ExportController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult All()
        {
            //
            //https://blog.stephencleary.com/2016/11/streaming-zip-on-aspnet-core.html
            //
            var result = new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"), async (outputStream, _) =>
            {
                var products = await db.Products.Include(p => p.Specs).AsNoTracking().ToListAsync();
                var benchmarks = await db.Benchmarks.AsNoTracking().ToListAsync();
                var specs = await db.Specifications.AsNoTracking().ToListAsync();
                var sources = await db.BenchmarkSources.AsNoTracking().Include(s => s.BenchmarkEntries).ToListAsync();

                var jsonSettings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };

                var names = sources.Select(s => $"sources/{s.Name}.json").ToList();
                var jsonItems = sources.Select(s => JsonConvert.SerializeObject(s, jsonSettings)).ToList();

                jsonItems.Add(JsonConvert.SerializeObject(products, jsonSettings));
                names.Add("products.json");

                jsonItems.Add(JsonConvert.SerializeObject(benchmarks, jsonSettings));
                names.Add("benchmarks.json");

                jsonItems.Add(JsonConvert.SerializeObject(specs, jsonSettings));
                names.Add("specifications.json");

                using (var zipArchive = new ZipArchive(new WriteOnlyStreamWrapper(outputStream), ZipArchiveMode.Create))
                {
                    for(int i = 0; i < names.Count; i++)
                    {
                        var name = names[i];
                        var json = jsonItems[i];

                        var zipEntry = zipArchive.CreateEntry(name);

                        using (var zipStream = zipEntry.Open())
                        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                        {
                            await stream.CopyToAsync(zipStream);
                        }
                    }
                }
            })
            {
                FileDownloadName = "MetaBenchmark.zip"
            };

            return result;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<BenchmarkSource>>> Sources()
        {
            return await db.BenchmarkSources
                .Include(b => b.BenchmarkEntries)
                .ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<Benchmark>>> Benchmarks()
        {
            return await db.Benchmarks.ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<BenchmarkEntry>>> BenchmarkEntries()
        {
            return await db.BenchmarkEntries.ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<Product>>> Products()
        {
            return await db.Products.ToListAsync();
        }
    }
}