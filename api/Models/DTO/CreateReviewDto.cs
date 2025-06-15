namespace api.Models.DTO
{
    public class CreateReviewDto
    {

        public int ResidentId { get; set; }
        public int? SpaceId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
