
namespace api.Dtos
{
    public class CreateSpaceDto
    {
        public string SpaceName { get; set; }
        public int Capacity { get; set; }
        public bool Availability { get; set; }
        public int? SpaceRuleId { get; set; }
        public string? ImageBase64 { get; set; } 

    }
}
