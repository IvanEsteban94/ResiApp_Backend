using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models.DTO
{
    public class ReservationDto : IValidatableObject
    {
        [Required(ErrorMessage = "El campo StartTime es obligatorio.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "El campo EndTime es obligatorio.")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "El campo ResidentId es obligatorio.")]
        public int ResidentId { get; set; }

        [Required(ErrorMessage = "El campo SpaceId es obligatorio.")]
        public int SpaceId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "El campo EndTime debe ser mayor que StartTime.",
                    new[] { nameof(EndTime) }
                );
            }
        }
    }
}
