namespace Dental_Clinic_NET.API.DTO
{
    public class AppointmentDocumentDTO
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string Title { get; set; }
        public MediaFileDTO Document { get; set; }
        public string Tag { get; set; }
    }
}
