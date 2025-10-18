using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using back_end.domain.enums;

namespace back_end.DTO.OrdersDTOs
{
    public class OrderItemCreateDTO
    {
        [JsonPropertyName("order_id")]
        public int Order_Id { get; set; }
        [JsonPropertyName("menu_id")]
        public int Menu_Id { get; set; }
        [JsonPropertyName("item_id")]
        public int Item_Id { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price_at_time")]
        public decimal Price_At_Time { get; set; }

        [JsonPropertyName("status")]
        public OrderStatus Status { get; set; }

    }

    public class OrderItemResponseDTO
    {
        [JsonPropertyName("order_item_id")]
        public int Order_Item_Id { get; set; }

        [JsonPropertyName("order_id")]
        public int Order_Id { get; set; }

        [JsonPropertyName("menu_id")]
        public int Menu_Id { get; set; }

        [JsonPropertyName("item_id")]
        public int Item_Id { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price_at_time")]
        public decimal Price_At_Time { get; set; }

        [JsonPropertyName("status")]
        public OrderStatus Status { get; set; }
        [JsonPropertyName("completed_at")]
        public DateTime? Completed_At { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

    }
    public class OrderItemInsertResponse
    {
        [JsonPropertyName("order_item_id")]
        public int Order_Item_Id { get; set; }

        [JsonPropertyName("order_id")]
        public int Order_Id { get; set; }

        [JsonPropertyName("menu_id")]
        public int Menu_Id { get; set; }

        [JsonPropertyName("item_id")]
        public int Item_Id { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price_at_time")]
        public decimal Price_At_Time { get; set; }

        [JsonPropertyName("status")]
        public OrderStatus Status { get; set; }

    }
    public class OrderCreateDTO
    {
        [JsonPropertyName("session_id")]
        public int Session_Id { get; set; }

        [JsonPropertyName("bill_id")]
        public int Bill_Id { get; set; }
    }

    public class OrderUpdateDTO
    {
        [JsonPropertyName("status")]
        public OrderStatus Status { get; set; }
    }

    public class OrderResponseDTO
    {
        public int Order_Id { get; set; }
        public int Session_Id { get; set; }
        public int Bill_Id { get; set; }
        public int User_Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime? Completed_At { get; set; }
        public List<OrderItemResponseDTO> OrderItems { get; set; } = new();

        // Automatically calculated from OrderItems
        public int PendingItemsCount
        {
            get
            {
                if (OrderItems == null)
                    return 0;

                return OrderItems.Count(item => item.Status == OrderStatus.Pending);
            }
        }
    }
    
    public class OrderStatusResponseDTO
    {
        [JsonPropertyName("order_id")]
        public int Order_Id { get; set; }

        [JsonPropertyName("status")]
        public OrderStatus Status { get; set; }

        [JsonPropertyName("bill_id")]
        public int Bill_Id { get; set; }


    }
}