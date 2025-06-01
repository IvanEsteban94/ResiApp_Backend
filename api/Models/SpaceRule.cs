namespace api.Models
{
    public class SpaceRule
    {
        public int Id { get; set; }
        public string Rule { get; set; }

    
        public int SpaceId { get; set; }
        public Space Space { get; set; }
    }
}
