using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace back_end.domain.Entities
{
    [Table("order_item")]
    public class OrderItems
    {
        [Key]
        public int Order_Item_Id { get; set; }

        //TODO Foreign Key to session orders OrderId
        public int Order_Key { get; set; }

        //TODO Foreign Key to menu menu id

        public int Menu_Id { get; set; }

        // TODO Foreign Key to Menu Item Item Id
        public int Item_Id { get; set; }

        public int Quantity { get; set; }

        public decimal Price_At_Time { get; set; }

        public OrderStatus Order_Item { get; set; }

        public DateTime Completed_At { get; set; }

        

    }

}
