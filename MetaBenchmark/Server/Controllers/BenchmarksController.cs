using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MetaBenchmark.Shared;
using MetaBenchmark.Server.Data;

namespace MetaBenchmark.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BenchmarksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BenchmarksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Benchmarks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Benchmark>>> GetBenchmarks()
        {
            return await _context.Benchmarks.ToListAsync();
        }

        // GET: api/Benchmarks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Benchmark>> GetBenchmark(long id)
        {
            var benchmark = await _context.Benchmarks.FindAsync(id);

            if (benchmark == null)
            {
                return NotFound();
            }

            return benchmark;
        }

        // PUT: api/Benchmarks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBenchmark(long id, Benchmark benchmark)
        {
            if (id != benchmark.ID)
            {
                return BadRequest();
            }

            _context.Entry(benchmark).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BenchmarkExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Benchmarks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Benchmark>> PostBenchmark(Benchmark benchmark)
        {
            _context.Benchmarks.Add(benchmark);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBenchmark", new { id = benchmark.ID }, benchmark);
        }

        // DELETE: api/Benchmarks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBenchmark(long id)
        {
            var benchmark = await _context.Benchmarks.FindAsync(id);
            if (benchmark == null)
            {
                return NotFound();
            }

            _context.Benchmarks.Remove(benchmark);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BenchmarkExists(long id)
        {
            return _context.Benchmarks.Any(e => e.ID == id);
        }
    }
}
