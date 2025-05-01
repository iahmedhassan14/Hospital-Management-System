using Hospital_Management_System.Data;
using Hospital_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management_System.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
            var appointment = _context.Appointments.Include(a => a.Doctor)
                                .ToList();

            return View(appointment.ToList());
        }
    }
}
