﻿using System.ComponentModel.DataAnnotations;

namespace api.Models.DTO
{
    public class ChangePasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

     
        public string NewPassword { get; set; } = null!;

       
    }
}
