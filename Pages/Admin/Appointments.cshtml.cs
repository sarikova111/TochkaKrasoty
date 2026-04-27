using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TochkaKrasoty.Data;
using TochkaKrasoty.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TochkaKrasoty.Pages.Admin
{
    public class AppointmentsModel : PageModel
    {
        private readonly SalonDbContext _context;

        public AppointmentsModel(SalonDbContext context)
        {
            _context = context;
        }

        public List<Appointment> Appointments { get; set; } = new();

        public SelectList MastersList { get; set; } = default!;
        public SelectList ServicesList { get; set; } = default!;
        public SelectList UsersList { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty]
        public int DeleteId { get; set; }

        public class InputModel
        {
            public int UserId { get; set; }
            public string ClientName { get; set; } = "";
            public string Phone { get; set; } = "";
            public int MasterId { get; set; }
            public int ServiceItemId { get; set; }
            public DateTime AppointmentDate { get; set; } = new DateTime(2026, 1, 1, 7, 0, 0);
        }

        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToPage("/Login");
            }

            LoadData();
            LoadLists();

            if (Input.AppointmentDate == default)
            {
                Input.AppointmentDate = new DateTime(2026, 1, 1, 7, 0, 0);
            }

            return Page();
        }

        public IActionResult OnPostAdd()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToPage("/Login");
            }

            LoadData();
            LoadLists();

            if (Input.UserId == 0 ||
                string.IsNullOrWhiteSpace(Input.ClientName) ||
                string.IsNullOrWhiteSpace(Input.Phone) ||
                Input.MasterId == 0 ||
                Input.ServiceItemId == 0 ||
                Input.AppointmentDate == default)
            {
                ModelState.AddModelError(string.Empty, "Заполните все поля");
                return Page();
            }

            if (Input.AppointmentDate.Year < 2026)
            {
                ModelState.AddModelError(string.Empty, "Запись возможна только начиная с 2026 года");
                return Page();
            }

            var time = Input.AppointmentDate.TimeOfDay;
            var minTime = new TimeSpan(7, 0, 0);
            var maxTime = new TimeSpan(19, 0, 0);

            if (time < minTime || time > maxTime)
            {
                ModelState.AddModelError(string.Empty, "Запись возможна только с 07:00 до 19:00");
                return Page();
            }

            if (Input.AppointmentDate.Minute != 0)
            {
                ModelState.AddModelError(string.Empty, "Запись возможна только на целый час");
                return Page();
            }

            var isBusy = _context.Appointments.Any(a =>
                a.MasterId == Input.MasterId &&
                a.AppointmentDate == Input.AppointmentDate);

            if (isBusy)
            {
                ModelState.AddModelError(string.Empty, "Это время уже занято");
                return Page();
            }

            var appointment = new Appointment
            {
                UserId = Input.UserId,
                ClientName = Input.ClientName.Trim(),
                Phone = Input.Phone.Trim(),
                MasterId = Input.MasterId,
                ServiceItemId = Input.ServiceItemId,
                AppointmentDate = Input.AppointmentDate
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToPage("/Login");
            }

            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == DeleteId);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        private void LoadData()
        {
            Appointments = _context.Appointments
                .Include(a => a.Master)
                .Include(a => a.Service)
                .Include(a => a.User)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        private void LoadLists()
        {
            UsersList = new SelectList(
                _context.Users.Where(u => u.Role == "Client").ToList(),
                "Id",
                "FullName"
            );

            MastersList = new SelectList(_context.Masters.ToList(), "Id", "FullName");
            ServicesList = new SelectList(_context.Services.ToList(), "Id", "Name");
        }
    }
}