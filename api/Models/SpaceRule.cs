namespace api.Models
{
    public class SpaceRule
    {
        public int Id { get; set; }
        public string Rule { get; set; }

        public ICollection<Space> Spaces { get; set; } = new List<Space>();
    }
}
