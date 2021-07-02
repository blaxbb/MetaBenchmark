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
    public class BenchmarkEntriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BenchmarkEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BenchmarkEntries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BenchmarkEntry>>> GetBenchmarkEntries()
        {
            return await _context.BenchmarkEntries.ToListAsync();
        }

        // GET: api/BenchmarkEntries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BenchmarkEntry>> GetBenchmarkEntry(long id)
        {
            var benchmarkEntry = await _context.BenchmarkEntries.FindAsync(id);

            if (benchmarkEntry == null)
            {
                return NotFound();
            }

            return benchmarkEntry;
        }

        // PUT: api/BenchmarkEntries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBenchmarkEntry(long id, BenchmarkEntry benchmarkEntry)
        {
            if (id != benchmarkEntry.Id)
            {
                return BadRequest();
            }

            _context.Entry(benchmarkEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BenchmarkEntryExists(id))
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

        // POST: api/BenchmarkEntries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BenchmarkEntry>> PostBenchmarkEntry(BenchmarkEntry benchmarkEntry)
        {
            _context.BenchmarkEntries.Add(benchmarkEntry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBenchmarkEntry", new { id = benchmarkEntry.Id }, benchmarkEntry);
        }

        // DELETE: api/BenchmarkEntries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBenchmarkEntry(long id)
        {
            var benchmarkEntry = await _context.BenchmarkEntries.FindAsync(id);
            if (benchmarkEntry == null)
            {
                return NotFound();
            }

            _context.BenchmarkEntries.Remove(benchmarkEntry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BenchmarkEntryExists(long id)
        {
            return _context.BenchmarkEntries.Any(e => e.Id == id);
        }
    }
}
