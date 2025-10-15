using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using back_end.domain;
using Microsoft.AspNetCore.SignalR;


namespace back_end.DTO.MenuItemAssignmentDTO
{
    public class MenuItemAssignmentBaseDTO
    {
        public int Menu_Id { get; set; }
        public int Item_Id { get; set; }
        // Add Annotation to be greater then 0
        public decimal Price { get; set; }
        public int? Total_Units_Ordered { get; set; } = 0;
        public int? Total_Views { get; set; } = 0;
        public int? Total_View_Seconds { get; set; } = 0;
        public int? Adult_Limit { get; set; } = 0;
        public int? Child_Limit { get; set; } = 0;
        public int? Senior_Limit { get; set; } = 0;
        public int? Tot_Limit { get; set; } = 0;

    }

    public class MenuAssignmentCreate : MenuItemAssignmentBaseDTO
    {
        public MenuItemStatus Status { get; set; } = MenuItemStatus.Available;
    }

    public class MenuAssignmentUpdateDTO
    {
        // Add Annotation to be greater then 0
        public decimal Price { get; set; }
        public int? Total_Units_Ordered { get; set; }
        public int? Total_Views { get; set; }
        public int? Total_View_Seconds { get; set; }
        public DateTime? Last_Ordered_At { get; set; }
        public DateTime? Last_Viewed_At { get; set; }
        public int? Adult_Limit { get; set; }
        public int? Child_Limit { get; set; }
        public int? Senior_Limit { get; set; }
        public int? Tot_Limit { get; set; }
        public MenuItemStatus Status { get; set; } = MenuItemStatus.Available;
        public bool Is_Add_on { get; set; }
    }

    class MenuAssignmentResponse : MenuItemAssignmentBaseDTO
    {
        public DateTime? Last_Ordered_At { get; set; }
        public DateTime? Last_Viewed_At { get; set; }
        public bool? Is_Add_On { get; set; }
        public MenuItemStatus Status { get; set; } = MenuItemStatus.Available; 
    }

}