﻿using System.ComponentModel.DataAnnotations;

namespace Users.Api.Models
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        
        public string Photo { get; set; }
    }
}