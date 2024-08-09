namespace PatientApiClient.Models
{
    public class Patient
    {
        public int MedicalRecordNumber { get; set; }  // This is now the ID
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public string[]? Contacts { get; set; }
        public string? AdmittingDiagnosis { get; set; }
        public string? AttendingPhysician { get; set; }
        public string? Department { get; set; }
    }
}
