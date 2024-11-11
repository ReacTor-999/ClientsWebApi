﻿using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Dto
{
    public record AccountLoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

    }
}