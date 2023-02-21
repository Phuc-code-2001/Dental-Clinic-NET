namespace Dental_Clinic_NET.API.DTO
{
    public class DocumentDTO
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string Title { get; set; }
        public MediaFileDTO File { get; set; }
        public string Tag { get; set; }
    }
}
