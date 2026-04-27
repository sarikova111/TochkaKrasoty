using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TochkaKrasoty.Data;
using TochkaKrasoty.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TochkaKrasoty.Pages
{
    public class AppointmentModel : PageModel
    {
        private readonly SalonDbContext _context;

        public AppointmentModel(SalonDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public SelectList MastersList { get; set; } = default!;
        public SelectList ServicesList { get; set; } = default!;
        public List<string> AvailableTimes { get; set; } = new();

        public class InputModel
        {
            public string ClientName { get; set; } = "";
            public string Phone { get; set; } = "";
            public int MasterId { get; set; }
            public int ServiceItemId { get; set; }
            public DateTime AppointmentDate { get; set; }
            public string AppointmentTime { get; set; } = "";
        }

        public IActionResult OnGet(int? masterId, DateTime? appointmentDate)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
                return RedirectToPage("/Login");

            if (userRole == "Admin")
                return RedirectToPage("/Admin/Dashboard");

            LoadLists();

            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
            if (user != null)
            {
                Input.ClientName = user.FullName;
                Input.Phone = user.Phone ?? "";
            }

            if (masterId.HasValue)
                Input.MasterId = masterId.Value;

            if (appointmentDate.HasValue)
                Input.AppointmentDate = appointmentDate.Value.Date;
            else
                Input.AppointmentDate = DateTime.Today;

            if (Input.MasterId > 0)
                LoadAvailableTimes(Input.MasterId, Input.AppointmentDate);

            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
                return RedirectToPage("/Login");

            if (userRole == "Admin")
                return RedirectToPage("/Admin/Dashboard");

            LoadLists();

            if (string.IsNullOrWhiteSpace(Input.ClientName) ||
                string.IsNullOrWhiteSpace(Input.Phone) ||
                Input.MasterId == 0 ||
                Input.ServiceItemId == 0 ||
                Input.AppointmentDate == default ||
                string.IsNullOrWhiteSpace(Input.AppointmentTime))
            {
                ModelState.AddModelError(string.Empty, "Заполните все поля");
                LoadAvailableTimes(Input.MasterId, Input.AppointmentDate);
                return Page();
            }

            if (Input.AppointmentDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(string.Empty, "Нельзя записаться на прошедшую дату");
                LoadAvailableTimes(Input.MasterId, Input.AppointmentDate);
                return Page();
            }

            if (!TimeSpan.TryParse(Input.AppointmentTime, out var selectedTime))
            {
                ModelState.AddModelError(string.Empty, "Выберите корректное время");
                LoadAvailableTimes(Input.MasterId, Input.AppointmentDate);
                return Page();
            }

            var minTime = new TimeSpan(7, 0, 0);
            var maxTime = new TimeSpan(19, 0, 0);

            if (selectedTime < minTime || selectedTime > maxTime)
            {
                ModelState.AddModelError(string.Empty, "Запись возможна только с 07:00 до 19:00");
                LoadAvailableTimes(Input.MasterId, Input.AppointmentDate);
                return Page();
            }

            var fullDateTime = Input.AppointmentDate.Date.Add(selectedTime);

            var isBusy = _context.Appointments.Any(a =>
                a.MasterId == Input.MasterId &&
                a.AppointmentDate == fullDateTime);

            if (isBusy)
            {
                ModelState.AddModelError(string.Empty, "Запись занята. Выберите другое время.");
                LoadAvailableTimes(Input.MasterId, Input.AppointmentDate);
                return Page();
            }

            var appointment = new Appointment
            {
                UserId = userId.Value,
                ClientName = Input.ClientName.Trim(),
                Phone = Input.Phone.Trim(),
                MasterId = Input.MasterId,
                ServiceItemId = Input.ServiceItemId,
                AppointmentDate = fullDateTime
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToPage("/Profile");
        }

        private void LoadLists()
        {
            // Если в Master у тебя поле называется Name, замени FullName на Name
            MastersList = new SelectList(_context.Masters.ToList(), "Id", "FullName");
            ServicesList = new SelectList(_context.Services.ToList(), "Id", "Name");
        }

        private void LoadAvailableTimes(int masterId, DateTime date)
        {
            AvailableTimes = new List<string>();

            if (masterId == 0 || date == default)
                return;

            var occupiedTimes = _context.Appointments
                .Where(a => a.MasterId == masterId && a.AppointmentDate.Date == date.Date)
                .Select(a => a.AppointmentDate.ToString("HH:mm"))
                .ToList();

            for (int hour = 7; hour <= 19; hour++)
            {
                var time = new TimeSpan(hour, 0, 0).ToString(@"hh\:mm");

                if (!occupiedTimes.Contains(time))
                    AvailableTimes.Add(time);
            }
        }
    }
}