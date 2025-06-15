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
    [Route("/api/v1/[controller]")]
    public class SpaceRulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpaceRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SpaceRule([FromBody] CreateSpaceRuleDto dto)
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

            return CreatedAtAction(nameof(SpaceRule), new { id = rule.Id }, result);
        }


        [HttpGet("findSpaceRuleById/{id}")]
        public async Task<ActionResult<ReadSpaceRuleDto>> findSpaceRuleById(int id)
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
        public async Task<IActionResult> ModifySpaceRule(int id, [FromBody] UpdateSpaceRuleDto dto)
        {
            var rule = await _context.SpaceRule.FindAsync(id);
            if (rule == null)
                return NotFound(new { success = false, message = "Space rule not found." });

            if (!string.IsNullOrWhiteSpace(dto.Rule))
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
        public async Task<IActionResult> RemoveSpaceRule(int id)
        {
            var rule = await _context.SpaceRule.FindAsync(id);
            if (rule == null)
                return NotFound(new { success = false, message = "Space rule not found." });

            _context.SpaceRule.Remove(rule);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Space rule removed successfully." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadSpaceRuleDto>>> FindSpaceRule()
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
