using System.ComponentModel.DataAnnotations;

namespace api.Models.DTO
{
    public class CreateSpaceRuleDto
    {
        [Required]
        public string Rule { get; set; }
    }
}

