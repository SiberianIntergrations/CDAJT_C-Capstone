using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace back_end.domain.Entities
{
    [Table("session_order")]
    public class SessionOrder
    {
        [Key]
        public int Order_Id { get; set; }

        [ForeignKey(nameof(DiningSession))]
        public int session_id { get; set; }

        [ForeignKey(nameof(Bill))]
        public int Bill_Id { get; set; }

        [ForeignKey(nameof(User))]
        public int User_Id { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime? Completed_At { get; set; }


        public DiningSession DiningSession { get; set; } = null!;
        public Billing Bill { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
        
    }

}
