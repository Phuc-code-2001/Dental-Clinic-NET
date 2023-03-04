namespace Dental_Clinic_NET.API.DTOs
{
    public class PatientDTO
    {
        public string Id { get; set; }
        public UserDTO BaseUser { get; set; }

        public MediaFileDTO MedicalRecordFile { get; set; }
    }

    public class PatientDTOLite
    {
        public string Id { get; set; }
        public UserDTOLite BaseUser { get; set; }
    }
}
