﻿using MetaBenchmark.Models;
using Newtonsoft.Json;

namespace MetaBenchmark
{
    public static class FileExport
    {
		static JsonSerializerSettings Settings => new JsonSerializerSettings()
		{
			Formatting = Formatting.Indented,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore
		};

		public static async Task<IEnumerable<(string filename, string contents)>> GetFiles(DataCache cache)
		{
			var all = await cache.All();
			List<(string filename, string text)> files = new List<(string, string)>();
			if (all != null && all.Value != null)
			{
				files.AddRange(ExportBenchmarkFiles(all.Value));
				files.AddRange(ExportSpecificationFiles(all.Value));
				files.AddRange(ExportSourceFiles(all.Value));
				files.AddRange(ExportProductFiles(all.Value));
			}

			return files;
		}

		public static IEnumerable<(string, string)> ExportBenchmarkFiles(IEnumerable<Product> all)
		{
			var benchmarkGroups = all.SelectMany(p => p.BenchmarkEntries).Select(e => e.Benchmark).Distinct().GroupBy(b => b.Type);
			foreach (var group in benchmarkGroups)
			{
				var type = group.Key;
				var items = group.ToList();
				items.ForEach(b => b.Type = Benchmark.BenchmarkType.FPS);
				var json = JsonConvert.SerializeObject(items.OrderBy(b => b.Id).ToList(), Settings);
				yield return ($"benchmarks/{type.ToString()}.json", json);
			}
		}

		public static IEnumerable<(string, string)> ExportSpecificationFiles(IEnumerable<Product> all)
		{
			foreach (var data in ExportProductSpecifications(all))
			{
				yield return data;
			}
			foreach (var data in ExportBenchmarkSpecifications(all))
			{
				yield return data;
			}
			foreach (var data in ExportBenchmarkEntrySpecifications(all))
			{
				yield return data;
			}
		}

		public static IEnumerable<(string, string)> ExportProductSpecifications(IEnumerable<Product> all)
		{
			var specGroups = all.SelectMany(p => p.Specs).Select(s => s.Spec).Distinct().GroupBy(s => s.Name);

			var indexJson = JsonConvert.SerializeObject(specGroups.Select(g => g.Key).ToList(), Settings);
			yield return ("specifications/Product/index.json", indexJson);

			foreach (var group in specGroups)
			{
				var name = group.Key;
				var items = group.ToList();
				items.ForEach(s =>
				{
					s.Name = default;
					s.Type = default;
				});
				var json = JsonConvert.SerializeObject(group.ToList(), Settings);
				yield return ($"specifications/Product/{name}.json", json);
			}
		}

		public static IEnumerable<(string, string)> ExportBenchmarkSpecifications(IEnumerable<Product> all)
		{
			var specGroups = all.SelectMany(p => p.BenchmarkEntries).Select(b => b.Benchmark).Where(b => b.Specs != null).SelectMany(b => b.Specs).Select(s => s.Spec).Distinct().GroupBy(s => s.Name);

			var indexJson = JsonConvert.SerializeObject(specGroups.Select(g => g.Key).ToList(), Settings);
			yield return ("specifications/Benchmark/index.json", indexJson);

			foreach (var group in specGroups)
			{
				var name = group.Key;
				var items = group.ToList();
				items.ForEach(s =>
				{
					s.Name = default;
					s.Type = default;
				});
				var json = JsonConvert.SerializeObject(group.ToList(), Settings);
				yield return ($"specifications/Benchmark/{name}.json", json);
			}
		}

		public static IEnumerable<(string, string)> ExportBenchmarkEntrySpecifications(IEnumerable<Product> all)
		{
			var specGroups = all.SelectMany(p => p.BenchmarkEntries).Select(b => b.Benchmark).Where(b => b.Specs != null).SelectMany(b => b.Entries).SelectMany(b => b.Specs).Select(s => s.Spec).Distinct().GroupBy(s => s.Name);

			var indexJson = JsonConvert.SerializeObject(specGroups.Select(g => g.Key).ToList(), Settings);
			yield return ("specifications/BenchmarkEntry/index.json", indexJson);

			foreach (var group in specGroups)
			{
				var name = group.Key;
				var items = group.ToList();
				items.ForEach(s =>
				{
					s.Name = default;
					s.Type = default;
				});
				var json = JsonConvert.SerializeObject(group.ToList(), Settings);
				yield return ($"specifications/BenchmarkEntry/{name}.json", json);
			}
		}

		public static IEnumerable<(string, string)> ExportSourceFiles(IEnumerable<Product> all)
		{
			var sourceGroups = all.SelectMany(p => p.BenchmarkEntries).GroupBy(b => b.Source);

			var indexJson = JsonConvert.SerializeObject(sourceGroups.Select(g => g.Key.Name).OrderBy(s => s).ToList(), Settings);
			yield return ("sources/index.json", indexJson);

			foreach (var group in sourceGroups)
			{
				var source = group.Key;
				var items = group.ToList();
				items.ForEach(e =>
				{
					e.Benchmark = default;
					e.SourceId = 0;
				});
				source.BenchmarkEntries = items.OrderBy(b => b.ProductId).ThenBy(b => b.BenchmarkId).ThenBy(b => b.Value).ToList();
				var json = JsonConvert.SerializeObject(source, Settings);
				yield return ($"sources/{source.Name}.json", json);
			}
		}

		public static IEnumerable<(string, string)> ExportProductFiles(IEnumerable<Product> all)
		{
			var sorted = all.OrderBy(p => p.Id).ToList();
			sorted.ForEach(p =>
			{
				p.BenchmarkEntries = null;
				p.Specs = p.Specs.Select(s => new SpecificationEntry() { SpecId = s.SpecId }).ToList();
			});
			var json = JsonConvert.SerializeObject(sorted, Settings);
			yield return ($"products.json", json);
		}

	}
}
