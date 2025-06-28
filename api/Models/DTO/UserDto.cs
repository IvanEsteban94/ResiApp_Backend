namespace api.Models.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? ResidentName { get; set; }
        public string? ApartmentInformation { get; set; }
        public string? SecurityWord { get; set; }
        // Nueva propiedad para la imagen en base64
        public string? ImageBase64
        {
            get; set;
        }

    }
}
