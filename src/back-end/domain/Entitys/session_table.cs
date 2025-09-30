using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace back_end.domain.Entities
{
    [Table("sessions")]
    public class Sessions
    {
        [ForeignKey(nameof(DiningSession))]
        public int Session_Id { get; set; }
        
        [ForeignKey(nameof(Table))]
        public int Table_Id { get; set; }


        public DiningSession DiningSession { get; set; } = null!;
        public TableEntity Table { get; set; } = null!;
    }
}
