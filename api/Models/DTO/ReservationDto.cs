
using System.ComponentModel.DataAnnotations;

namespace api.Models.DTO
{
    public class ReservationDto : IValidatableObject
    {
        [Required(ErrorMessage = "The StartTime field is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "The EndTime field is required.")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "The ResidentId field is required.")]
        public int ResidentId { get; set; }

        [Required(ErrorMessage = "The SpaceId field is required.")]
        public int SpaceId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "The EndTime field must be greater than StartTime.",
                    new[] { nameof(EndTime) }
                );
            }
        }
    }
}
