namespace api.Models.DTO
{
    public class UpdateReviewDto
    {
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public int? ResidentId { get; set; }
        public int? SpaceId { get; set; }
    }
}
