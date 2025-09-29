using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.SignalR;
namespace back_end.domain.Entities
{
    [Table("bills")]
    public class Billing
    {
        [Key]
        public int Bill_Id { get; set; }

        //Foreign Key to Dinning Session, Session ID
        public int Session_Id { get; set; }
        [MaxLength(255)]
        public string Bill_Name { get; set; } = string.Empty;

        public int Senior_Count { get; set; }

        public int Adult_Count { get; set; }

        public int Child_Count { get; set; }

        public int Total_Count { get; set; }

        public DateTime Status { get; set; } = DateTime.Now;

        public DateTime Created_At { get; set; }
    }

}