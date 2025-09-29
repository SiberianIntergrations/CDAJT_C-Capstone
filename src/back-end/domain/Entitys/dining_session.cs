using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace back_end.domain.Entities
{
    [Table("dining_sessions")]

    public class DiningSession
    {
        [Key]
        public int Session_Id { get; set; }
        //TODO Foreign Key Assignment to Menu, Menu.Id
        public int Menu_Id { get; set; }

        public int Started_At { get; set; }

        public int Ended_At { get; set; }

        public DateTime First_Order_At { get; set; } = DateTime.Now;

    }

}