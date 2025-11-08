using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
namespace back_end.domain.Entities
{
    [Table("session_participant")]
    public class SessionParticipant
    {
        [Key]
        public int Participant_Id { get; set; }

        [ForeignKey(nameof(DiningSession))]
        public int Session_Id { get; set; }
        
        // Replacing User_Id with User_Oid and User_Name
        [MaxLength(100)]
        public string? User_Oid { get; set; }
        [MaxLength(200)]
        public string? User_Name { get; set; }

        public DateTime Joined_At { get; set; } = DateTime.Now;

        public DateTime? Left_At { get; set; }


        public DiningSession DiningSession { get; set; } = null!;
        // Removed: public User? User { get; set; } = null!;
    }

}