using System;
using System.ComponentModel.DataAnnotations;
using back_end.domain;

namespace back_end.DTO.UserDTOs
{
    public class UserCreateDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string First_name { get; set; } = string.Empty;

        [Required]
        public string Last_name { get; set; } = string.Empty;
    }

    public class UserResponseDTO
    {
        public int User_id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string First_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        public UserRoles Role { get; set; }
        public bool Is_email_confirmed { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Last_interaction_at { get; set; }
    }

    public class StaffUpdateDTO
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? First_name { get; set; }
        public string? Last_name { get; set; }
        public bool? Is_email_confirmed { get; set; }
        public UserRoles? Role { get; set; }
    }
}