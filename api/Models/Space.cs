namespace api.Models
{
    public class Space
    {
        public int Id { get; set; }
        public string SpaceName { get; set; }
        public int Capacity { get; set; }
        public bool Availability { get; set; }

        public int? SpaceRuleId { get; set; }
        public SpaceRule? SpaceRule { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        // Nueva propiedad para la imagen en base64
        public string? ImageBase64
        {
            get; set;
        }
}
}