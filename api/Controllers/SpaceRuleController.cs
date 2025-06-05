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
            var rule = new SpaceRule
            {
                Rule = dto.Rule
            };

            try
            {
                _context.SpaceRule.Add(rule);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al guardar regla en la base de datos.", detail = ex.Message });
            }

            var result = new ReadSpaceRuleDto
            {
                Id = rule.Id,
                Rule = rule.Rule
            };

            return CreatedAtAction(nameof(ReadSpaceRule), new { id = rule.Id }, result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ReadSpaceRuleDto>> ReadSpaceRule(int id)
        {
            var rule = await _context.SpaceRule.FindAsync(id);
            if (rule == null) return NotFound();

            var result = new ReadSpaceRuleDto
            {
                Id = rule.Id,
                Rule = rule.Rule
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpaceRule(int id, [FromBody] UpdateSpaceRuleDto dto)
        {
            var rule = await _context.SpaceRule.FindAsync(id);
            if (rule == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Rule))
                rule.Rule = dto.Rule;

            await _context.SaveChangesAsync();

            var result = new ReadSpaceRuleDto
            {
                Id = rule.Id,
                Rule = rule.Rule
            };

            return Ok(result);
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadSpaceRuleDto>>> GetAllSpaceRules()
        {
            var rules = await _context.SpaceRule.ToListAsync();

            var result = rules.ConvertAll(rule => new ReadSpaceRuleDto
            {
                Id = rule.Id,
                Rule = rule.Rule
            });

            return Ok(result);
        }
    }
}
