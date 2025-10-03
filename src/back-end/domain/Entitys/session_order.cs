using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace back_end.domain.Entities
{
    [Table("session_order")]
    public class SessionOrder
    {
        [Key]
        public int Order_Id { get; set; }

        //TODO Foreign Key to dinning session session id
        public int session_id { get; set; }

        //TODO Foreign key to bill, Billing Id
        public int Bill_Id { get; set; }

        //TODO Foreign key to User Id
        public int User_Id { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Completed_At { get; set; }
        
    }

}
