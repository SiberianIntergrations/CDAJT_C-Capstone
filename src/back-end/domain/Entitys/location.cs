using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
namespace back_end.domain.Entities
{
    [Table("locations")]

    public class Locations
    {
        [Key]
        public int Location_Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Address_Primary { get; set; } = string.Empty;

        public string Address_Secondary { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Province { get; set; } = string.Empty;

        public string Postal_Code { get; set; } = string.Empty;

        public string Phone_Number { get; set; } = string.Empty;

        public DateTime Created_At { get; set; } = DateTime.Now;
        
        
        public DateTime Updated_At { get; set; } 
    }

}