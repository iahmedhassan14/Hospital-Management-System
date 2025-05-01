using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Message { get; set; }
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
        
        
    }
}
