using api.Models;
using api.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Controllers
{
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
        public async Task<IActionResult> CreateSpaceRule([FromBody] CreateSpaceRuleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rule = new SpaceRule
            {
                Rule = dto.Rule,
                SpaceId = dto.SpaceId
            };

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpaceRule(int id)
        {
            var rule = await _context.SpaceRule.FindAsync(id);
            if (rule == null)
                return NotFound(new { success = false, message = "Space rule not found." });

            _context.SpaceRule.Remove(rule);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Space rule deleted successfully." });
        }

        [HttpGet("rules")]
        public async Task<ActionResult<IEnumerable<SpaceRule>>> GetAllSpaceRules()
        {
            var rules = await _context.SpaceRule.ToListAsync();
            return Ok(rules);
        }

        
    }
}
