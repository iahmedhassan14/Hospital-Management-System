using Hospital_Management_System.Models;

namespace Hospital_Management_System.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        public List<Appointment>? Appointments { get; set; } 
    }
}