using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace back_end.domain.Entities
{
    [Table("User")]

    public class User
    {
        [Key]
        public int User_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Normalized_email { get => Email.ToUpper(); private set { } }

        [Required]
        [MaxLength(255)]
        public string Password_hash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string First_name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Last_name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public UserRoles Role { get; set; } = UserRoles.Customer;

        public UserStatus Status { get; set; } = UserStatus.Active;

        public DateTime Created_at { get; set; } = DateTime.Now;

        public DateTime? Last_Interaction_at { get; set; }
        
        public bool Is_email_confirmed { get; set; } = false;

    }
}