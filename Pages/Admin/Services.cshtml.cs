using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TochkaKrasoty.Data;
using TochkaKrasoty.Models;

namespace TochkaKrasoty.Pages.Admin
{
    public class ServicesModel : PageModel
    {
        private readonly SalonDbContext _context;

        public ServicesModel(SalonDbContext context)
        {
            _context = context;
        }

        public List<ServiceItem> Services { get; set; } = new();
        public List<Master> Masters { get; set; } = new();

        [BindProperty]
        public ServiceItem NewService { get; set; } = new();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToPage("/Login");

            Masters = _context.Masters.ToList();
            Services = _context.Services
                .Include(s => s.Master)
                .ToList();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToPage("/Login");

            Masters = _context.Masters.ToList();
            Services = _context.Services
                .Include(s => s.Master)
                .ToList();

            if (NewService.MasterId == 0)
            {
                ModelState.AddModelError(string.Empty, "Выберите мастера");
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewService.Name))
            {
                ModelState.AddModelError(string.Empty, "Введите название процедуры");
                return Page();
            }

            _context.Services.Add(NewService);
            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToPage("/Login");

            var service = _context.Services.FirstOrDefault(s => s.Id == id);
            if (service != null)
            {
                _context.Services.Remove(service);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}