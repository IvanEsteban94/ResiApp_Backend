namespace api.Models.DTO
{
    public class UpdateSpaceDto
    {
        public string SpaceName { get; set; }
        public int? Capacity { get; set; }
        public bool? Availability { get; set; }
        public int? SpaceRuleId { get; set; }
    }
}
