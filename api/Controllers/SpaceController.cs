using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using System;

namespace api.Controllers
{
    // Controllers/SpaceController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class SpaceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpaceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpace([FromBody] Space space)
        {
            _context.Space.Add(space);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(ReadSpace), new { id = space.Id }, space);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Space>> ReadSpace(int id)
        {
            var space = await _context.Space
                                      .Include(s => s.SpaceRules)
                                      .FirstOrDefaultAsync(s => s.Id == id);
            if (space == null) return NotFound();
            return space;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpace(int id, [FromBody] Space updatedSpace)
        {
            if (id != updatedSpace.Id) return BadRequest();

            _context.Entry(updatedSpace).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

