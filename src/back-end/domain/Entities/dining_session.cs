using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace back_end.domain.Entities
{
    [Table("dining_sessions")]

    public class DiningSession
    {
        [Key]
        public int Session_Id { get; set; }


        [ForeignKey(nameof(Menu))]
        public int Menu_Id { get; set; }

        public DateTime Started_At { get; set; }

        public DateTime? Ended_At { get; set; }

        public DateTime First_Order_At { get; set; } = DateTime.Now;



        public Menu Menu { get; set; } = null!;
        public ICollection<Billing> Bills { get; set; } = new List<Billing>();
        public ICollection<SessionOrder> Orders { get; set; } = new List<SessionOrder>();
        public ICollection<SessionParticipant> Participants { get; set; } = new List<SessionParticipant>();
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public ICollection<Sessions> Sessions { get; set; } = new List<Sessions>();

    }

}