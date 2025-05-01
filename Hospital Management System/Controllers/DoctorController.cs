using Hospital_Management_System.Data;
using Hospital_Management_System.Models.ViewModels;
using Hospital_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management_System.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index(string? search, int page = 1)
        {
            IQueryable<Doctor> doctor = _context.Doctors;

            #region Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                doctor = doctor.Where(e =>
                    e.Name.Contains(search) ||
                    e.Specialization.Contains(search));
            }
            #endregion

            #region Pagination
            int pageSize = 6;
            var totalDoctors = doctor.Count();
            doctor = doctor.Skip((page - 1) * pageSize).Take(pageSize);
            
            var totalPageNum = Math.Ceiling(totalDoctors / (double)pageSize);
            #endregion

            DoctorVM doctorVM = new()
            {
                Doctor = doctor.ToList(),
                TotalPageNum = totalPageNum,
                Search = search,
                CurrentPage = page
            };

            return View(doctorVM);
        }
        public IActionResult Book(int DoctorId)
        {
            IQueryable<Doctor> doctor = _context.Doctors;
            BookVM bookVM = new()
            { 
                    Doctors = doctor.ToList(),
                    DoctorId = DoctorId,
                    DoctorName = _context.Doctors.Find(DoctorId)?.Name,
            };
            return View(bookVM);
        }
        [HttpPost]
        public IActionResult Book(Appointment appointment)
        {
            #region Validation
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix the errors in the form.";
                var bookVm = new BookVM
                {
                    Appointment = appointment,
                    Doctors = _context.Doctors.ToList(),
                    DoctorId = appointment.DoctorId,
                    DoctorName = _context.Doctors.Find(appointment.DoctorId)?.Name
                };
                return View(bookVm);
            }

            var appointmentDateTime = appointment.Date;
            var dayOfWeek = appointmentDateTime.DayOfWeek;
            var hour = appointmentDateTime.Hour;
            var minute = appointmentDateTime.Minute;

            if (dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday)
            {
                TempData["Error"] = "Appointments are not available on Fridays and Saturdays.";
                var bookVm = new BookVM
                {
                    Appointment = appointment,
                    Doctors = _context.Doctors.ToList(),
                    DoctorId = appointment.DoctorId,
                    DoctorName = _context.Doctors.Find(appointment.DoctorId)?.Name
                };
                return View(bookVm);
            }

            if (hour < 9 || hour >= 21)
            {
                TempData["Error"] = "Appointments must be between 9:00 AM and 9:00 PM.";
                var bookVm = new BookVM
                {
                    Appointment = appointment,
                    Doctors = _context.Doctors.ToList(),
                    DoctorId = appointment.DoctorId,
                    DoctorName = _context.Doctors.Find(appointment.DoctorId)?.Name
                };
                return View(bookVm);
            }

            if (minute != 0 && minute != 30)
            {
                TempData["Error"] = "Appointments must start exactly at 00 or 30 minutes.";
                var bookVm = new BookVM
                {
                    Appointment = appointment,
                    Doctors = _context.Doctors.ToList(),
                    DoctorId = appointment.DoctorId,
                    DoctorName = _context.Doctors.Find(appointment.DoctorId)?.Name
                };
                return View(bookVm);
            }

            bool isDoctorBooked = _context.Appointments
                .Any(a => a.DoctorId == appointment.DoctorId && a.Date == appointment.Date);

            if (isDoctorBooked)
            {
                TempData["Error"] = "The selected time is already booked for this doctor. Please choose another time.";
                var bookVm = new BookVM
                {
                    Appointment = appointment,
                    Doctors = _context.Doctors.ToList(),
                    DoctorId = appointment.DoctorId,
                    DoctorName = _context.Doctors.Find(appointment.DoctorId)?.Name
                };
                return View(bookVm);
            }
            #endregion

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            TempData["Success"] = "Your appointment has been booked successfully!";
            return RedirectToAction("Index" , "Report");
        }

        public IActionResult Edit(int id)
        {
            var appointment = _context.Appointments
                              .Include(a => a.Doctor)
                              .FirstOrDefault(a => a.Id == id);

            var doctors = _context.Doctors.ToList();

            var bookVM = new BookVM
            {
                Appointment = appointment,
                Doctors = doctors
            };

            return View(bookVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookVM bookVM)
        {
            bookVM.Doctors = _context.Doctors.ToList();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix the form errors";
                return View(bookVM);
            }

            try
            {
                var existingAppointment = _context.Appointments.Find(bookVM.Appointment?.Id);
                if (existingAppointment == null)
                {
                    TempData["Error"] = "Appointment not found";
                    return RedirectToAction("Index" , "Report");
                }
                existingAppointment.Name = bookVM.Appointment.Name;
                existingAppointment.Phone = bookVM.Appointment.Phone;
                existingAppointment.Date = bookVM.Appointment.Date;
                existingAppointment.Message = bookVM.Appointment.Message;
                existingAppointment.DoctorId = bookVM.Appointment.DoctorId;

                _context.SaveChanges();
                TempData["Success"] = "Appointment updated successfully!";
                return RedirectToAction("Index", "Report");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating appointment: " + ex.Message;
                return View(bookVM);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var appointment = _context.Appointments
                .Include(a => a.Doctor)
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null)
            {
                TempData["Error"] = "Appointment not found";
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        [HttpPost]
        [ActionName("Delete")]
        [IgnoreAntiforgeryToken]
        public IActionResult ConfirmDelete(int id)
        {
            try
            {
                var appointment = _context.Appointments.Find(id);
                if (appointment == null)
                {
                    return Json(new { success = false, message = "Appointment not found" });
                }

                _context.Appointments.Remove(appointment);
                _context.SaveChanges();

                return Json(new { success = true, message = "Appointment deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
