
namespace api.Models.DTO
{
    public class ReadSpaceDto
    {

        public int Id { get; set; }
        public string SpaceName { get; set; }
        public int Capacity { get; set; }
        public bool Availability { get; set; }
        
        public List<ReadSpaceRuleDto> SpaceRules { get; set; }
    }
}
