namespace Hospital_Management_System.Models.ViewModels
{
    public class DoctorVM
    {
        public List<Doctor> Doctor { get; set; } = new();
        public double TotalPageNum { get; set; }
        public string? Search { get; set; }
        public int CurrentPage { get; set; }

    }
}
