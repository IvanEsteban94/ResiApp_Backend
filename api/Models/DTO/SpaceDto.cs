namespace api.Dtos
{
    public class SpaceDto
    {
        public int Id { get; set; }
        public string SpaceName { get; set; } = null!;
        public int? Capacity { get; set; } // <-- Asegúrate que esto sea nullable si lo es en la DB
        public bool Availability { get; set; }
        public int SpaceRuleId { get; set; }
    }
}
