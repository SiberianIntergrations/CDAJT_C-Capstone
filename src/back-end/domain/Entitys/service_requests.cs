using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace back_end.domain.Entities
{
    [Table("service_request")]
    public class ServiceRequest
    {
        [Key]
        public int request_id { get; set; }

        //TODO Forgein keyy to dinning session, session id
        public int Session_Id { get; set; }

        //TODO Foriegn Key to Table Id
        public int Table_Id { get; set; }

        //TODO Foreign key to user user id
        public int Request_By { get; set; }

        //TODO Foreign key to User, USer Id
        public int Claimed_By { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        public ServiceRequestStatus Status { get; set; }

        public DateTime Created_At { get; set; } = DateTime.Now;

        public DateTime Claimed_At { get; set; }

        public DateTime Completed_At { get; set; }
        

    }

}