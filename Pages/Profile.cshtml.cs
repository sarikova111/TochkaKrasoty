using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TochkaKrasoty.Data;
using TochkaKrasoty.Models;
using System.Collections.Generic;
using System.Linq;

namespace TochkaKrasoty.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly SalonDbContext _context;

        public ProfileModel(SalonDbContext context)
        {
            _context = context;
        }

        public User? CurrentUser { get; set; }
        public List<Appointment> MyAppointments { get; set; } = new();

        [BindProperty]
        public int AppointmentId { get; set; }

        [BindProperty]
        public string ReviewText { get; set; } = "";

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            if (userRole == "Admin")
            {
                return RedirectToPage("/Admin/Dashboard");
            }

            LoadData(userId.Value);
            return Page();
        }

        public IActionResult OnPostAddReview()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            if (userRole == "Admin")
            {
                return RedirectToPage("/Admin/Dashboard");
            }

            if (string.IsNullOrWhiteSpace(ReviewText))
            {
                LoadData(userId.Value);
                ModelState.AddModelError(string.Empty, "Введите отзыв");
                return Page();
            }

            var appointment = _context.Appointments.FirstOrDefault(a =>
                a.Id == AppointmentId && a.UserId == userId.Value);

            if (appointment == null)
            {
                LoadData(userId.Value);
                ModelState.AddModelError(string.Empty, "Запись не найдена");
                return Page();
            }

            appointment.Review = ReviewText.Trim();
            _context.SaveChanges();

            return RedirectToPage("/Profile");
        }

        private void LoadData(int userId)
        {
            CurrentUser = _context.Users.FirstOrDefault(u => u.Id == userId);

            MyAppointments = _context.Appointments
                .Include(a => a.Master)
                .Include(a => a.Service)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }
    }
}