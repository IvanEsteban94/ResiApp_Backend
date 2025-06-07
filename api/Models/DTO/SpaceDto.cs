using api.Models;
using api.Models.DTO;

namespace api.Dtos
{
    public class SpaceDto
    {
        public int Id { get; set; }
        public string SpaceName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool Availability { get; set; }

        // Lista de reglas
        public List<SpaceRule> SpaceRules { get; set; } = new();
    }
}
