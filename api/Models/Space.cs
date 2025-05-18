namespace api.Models
{
    public class Space
    {
        public int Id { get; set; }
        public string SpaceName { get; set; } = null!;
        public int Capacity { get; set; }
        public bool Availability { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
        public ICollection<SpaceRule>? SpaceRules { get; set; }
    }
}
