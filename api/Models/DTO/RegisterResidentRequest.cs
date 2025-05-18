using System.ComponentModel.DataAnnotations;

public record RegisterResidentRequest(
      [Required][EmailAddress] string Email,
      [Required][MinLength(6)] string Password,
      string? ResidentName,
      string? ApartmentInformation
  );
