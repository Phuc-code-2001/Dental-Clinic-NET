namespace Dental_Clinic_NET.API.DTO
{
    public class PatientDTO
    {
        public string Id { get; set; }
        public UserDTO BaseUser { get; set; }

        public MediaFileDTO MedicalRecordFile { get; set; }
    }
}
