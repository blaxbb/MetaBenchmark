using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MetaBenchmark.Server.Data;
using MetaBenchmark.Shared.Models;

namespace MetaBenchmark.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecificationEntriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpecificationEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SpecificationEntries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecificationEntry>>> GetSpecificationEntries()
        {
            return await _context.SpecificationEntries.ToListAsync();
        }

        // GET: api/SpecificationEntries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecificationEntry>> GetSpecificationEntry(long id)
        {
            var specificationEntry = await _context.SpecificationEntries.FindAsync(id);

            if (specificationEntry == null)
            {
                return NotFound();
            }

            return specificationEntry;
        }

        // PUT: api/SpecificationEntries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecificationEntry(long id, SpecificationEntry specificationEntry)
        {
            if (id != specificationEntry.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(specificationEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecificationEntryExists(id))
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

        // POST: api/SpecificationEntries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SpecificationEntry>> PostSpecificationEntry(SpecificationEntry specificationEntry)
        {
            _context.SpecificationEntries.Add(specificationEntry);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SpecificationEntryExists(specificationEntry.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSpecificationEntry", new { id = specificationEntry.ProductId }, specificationEntry);
        }

        // DELETE: api/SpecificationEntries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecificationEntry(long id)
        {
            var specificationEntry = await _context.SpecificationEntries.FindAsync(id);
            if (specificationEntry == null)
            {
                return NotFound();
            }

            _context.SpecificationEntries.Remove(specificationEntry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpecificationEntryExists(long id)
        {
            return _context.SpecificationEntries.Any(e => e.ProductId == id);
        }
    }
}
