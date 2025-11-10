namespace back_end.domain.DTOs
{
    public class CreateSessionRequestDTO
    {
        public int? Menu_Id { get; set; }
        public int? Location_Id { get; set; }
        public int? Table_Id { get; set; }
        public string? Request_By_Oid { get; set; }
        public string? Request_By_Name { get; set; }
    }
}
