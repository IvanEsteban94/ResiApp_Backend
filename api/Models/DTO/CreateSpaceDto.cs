using System.ComponentModel.DataAnnotations;

namespace api.Models.DTO
{
    public class CreateSpaceDto
    {
        public string SpaceName { get; set; }
        public int Capacity { get; set; }
        public bool Availability { get; set; }
        public int? SpaceRuleId { get; set; }

    }
}
