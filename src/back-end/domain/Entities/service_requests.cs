using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using back_end.domain.enums;

namespace back_end.domain.Entities
{
    [Table("service_request")]
    public class ServiceRequest
    {
        [Key]
        public int request_id { get; set; }

        [ForeignKey(nameof(DiningSession))]
        public int Session_Id { get; set; }

        [ForeignKey(nameof(Table))]
        public int Table_Id { get; set; }

        // Replacing Request_By and Claimed_By with Oid and Name
        [MaxLength(100)]
        public string Request_By_Oid { get; set; } = string.Empty;
        [MaxLength(200)]
        public string Request_By_Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Claimed_By_Oid { get; set; }
        [MaxLength(200)]
        public string? Claimed_By_Name { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        public ServiceRequestStatus Status { get; set; }

        public DateTime Created_At { get; set; } = DateTime.Now;

        public DateTime? Claimed_At { get; set; }

        public DateTime? Completed_At { get; set; }

        public DiningSession DiningSession { get; set; } = null!;
        public TableEntity Table { get; set; } = null!;
        // Removed: public User RequestedByUser { get; set; } = null!;
        // Removed: public User? ClaimedByUser { get; set; }
    }

}