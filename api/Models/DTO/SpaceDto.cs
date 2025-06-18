using api.Models;
using api.Models.DTO;

namespace api.Dtos
{
    public class SpaceDto
    {
        public int Id { get; set; }
        public string SpaceName { get; set; }
        public int Capacity { get; set; }
        public bool Availability { get; set; }
        public List<SpaceRule> SpaceRules { get; set; }

        public string? ImageBase64 { get; set; } 
    }
}
