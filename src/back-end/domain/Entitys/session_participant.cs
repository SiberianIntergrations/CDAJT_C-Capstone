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

        //Todo Requires a Foreign Key to dining session session id
        public int Session_Id { get; set; }
        //ToDo: Requires a foreign key to User UserId
        public int User_Id { get; set; }

        public DateTime Joined_At { get; set; }

        public DateTime Left_At { get; set; }

        
    }

}