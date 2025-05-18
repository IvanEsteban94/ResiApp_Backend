namespace api.Models
{
    public class SpaceRule
    {
        public int Id { get; set; }
        public string Rule { get; set; } = null!;
        public int SpaceId { get; set; }
    }
}
