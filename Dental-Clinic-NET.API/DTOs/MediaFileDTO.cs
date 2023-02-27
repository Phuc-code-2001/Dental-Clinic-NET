namespace Dental_Clinic_NET.API.DTOs
{
    public class MediaFileDTO : BaseEntityDTO
    {
        public int Id { get; set; }
        public string FileURL { get; set; }
        public string Category { get; set; }
    }
}
