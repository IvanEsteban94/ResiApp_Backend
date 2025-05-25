using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using System;

namespace api.Controllers
{
    // Controllers/SpaceRuleController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class SpaceRuleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpaceRuleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpaceRule([FromBody] SpaceRule rule)
        {
            _context.SpaceRule.Add(rule);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(ReadSpaceRule), new { id = rule.Id }, rule);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SpaceRule>> ReadSpaceRule(int id)
        {
            var rule = await _context.SpaceRule.FindAsync(id);
            if (rule == null) return NotFound();
            return rule;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpaceRule(int id, [FromBody] SpaceRule updatedRule)
        {
            if (id != updatedRule.Id) return BadRequest();

            _context.Entry(updatedRule).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

