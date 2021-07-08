using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MetaBenchmark.Server.Data;
using MetaBenchmark.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace MetaBenchmark.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BenchmarkSourcesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BenchmarkSourcesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BenchmarkSources
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BenchmarkSource>>> GetBenchmarkSources()
        {
            return await _context.BenchmarkSources.ToListAsync();
        }

        // GET: api/BenchmarkSources/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BenchmarkSource>> GetBenchmarkSource(long id)
        {
            var benchmarkSource = await _context.BenchmarkSources.FindAsync(id);

            if (benchmarkSource == null)
            {
                return NotFound();
            }

            return benchmarkSource;
        }

        // PUT: api/BenchmarkSources/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBenchmarkSource(long id, BenchmarkSource benchmarkSource)
        {
            if (id != benchmarkSource.Id)
            {
                return BadRequest();
            }

            _context.Entry(benchmarkSource).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BenchmarkSourceExists(id))
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

        // POST: api/BenchmarkSources
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BenchmarkSource>> PostBenchmarkSource(BenchmarkSource benchmarkSource)
        {
            _context.BenchmarkSources.Add(benchmarkSource);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBenchmarkSource", new { id = benchmarkSource.Id }, benchmarkSource);
        }

        // DELETE: api/BenchmarkSources/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBenchmarkSource(long id)
        {
            var benchmarkSource = await _context.BenchmarkSources.FindAsync(id);
            if (benchmarkSource == null)
            {
                return NotFound();
            }

            _context.BenchmarkSources.Remove(benchmarkSource);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BenchmarkSourceExists(long id)
        {
            return _context.BenchmarkSources.Any(e => e.Id == id);
        }
    }
}
