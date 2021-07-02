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
    public class SpecificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpecificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Specifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Specification>>> GetSpecifications()
        {
            return await _context.Specifications
                .Include(s => s.Products)
                .ToListAsync();
        }

        // GET: api/Specifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Specification>> GetSpecification(long id)
        {
            var specification = await _context.Specifications.FindAsync(id);

            if (specification == null)
            {
                return NotFound();
            }

            return specification;
        }

        // PUT: api/Specifications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecification(long id, Specification specification)
        {
            if (id != specification.Id)
            {
                return BadRequest();
            }

            _context.Entry(specification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecificationExists(id))
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

        // POST: api/Specifications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Specification>> PostSpecification(Specification specification)
        {
            _context.Specifications.Add(specification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecification", new { id = specification.Id }, specification);
        }

        // DELETE: api/Specifications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecification(long id)
        {
            var specification = await _context.Specifications.FindAsync(id);
            if (specification == null)
            {
                return NotFound();
            }

            _context.Specifications.Remove(specification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpecificationExists(long id)
        {
            return _context.Specifications.Any(e => e.Id == id);
        }
    }
}
