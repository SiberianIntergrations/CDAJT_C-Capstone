using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.SignalR;
namespace back_end.domain.Entities
{
    [Table("table_entity")]

    public class TableEntity
    {
        [Key]
        public int Table_Id { get; set; }

        [Required]
        public int table_number { get; set; }
        [MaxLength(255)]
        public string QR_Code { get; set; } = string.Empty;

        public int seat_count { get; set; }

        public bool is_active { get; set; } = false;
        
    }
}