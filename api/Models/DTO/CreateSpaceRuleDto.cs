namespace api.Models.DTO
{
    public class CreateSpaceRuleDto
    {
        public required string Rule { get; set; }
        public int SpaceId { get; set; }
    }
}

