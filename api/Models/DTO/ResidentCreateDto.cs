using System.ComponentModel.DataAnnotations;

namespace api.Models.DTO
{
    public class ResidentCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string ResidentName { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ser un correo electrónico válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; }

        [MaxLength(200, ErrorMessage = "La información del apartamento no puede superar los 200 caracteres.")]
        public string ApartmentInformation { get; set; }
    }
}
