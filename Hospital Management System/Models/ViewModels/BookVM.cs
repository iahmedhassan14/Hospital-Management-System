namespace Hospital_Management_System.Models.ViewModels
{
    public class BookVM
    {
        public List<Doctor> Doctors { get; set; } = new();
        public Appointment? Appointment { get; set; }
        public string? DoctorName { get; set; }
        public int DoctorId { get; set; }
    }
}
