

namespace api.Models.DTO
{
    public class Space
    {
        public int Id { get; set; }
        public string SpaceName { get; set; }
        public int Capacity { get; set; }
        public bool Availability { get; set; }

        // FK a SpaceRule
        public int? SpaceRuleId { get; set; }
        public SpaceRule SpaceRule { get; set; }
    }
}
