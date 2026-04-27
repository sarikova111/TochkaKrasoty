using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TochkaKrasoty.Data;
using TochkaKrasoty.Models;

namespace TochkaKrasoty.Pages.Admin
{
    public class MastersModel : PageModel
    {
        private readonly SalonDbContext _context;

        public MastersModel(SalonDbContext context)
        {
            _context = context;
        }

        public List<Master> Masters { get; set; } = new();

        [BindProperty]
        public Master NewMaster { get; set; } = new();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToPage("/Login");

            Masters = _context.Masters.ToList();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToPage("/Login");

            _context.Masters.Add(NewMaster);
            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToPage("/Login");

            var master = _context.Masters.FirstOrDefault(m => m.Id == id);
            if (master != null)
            {
                _context.Masters.Remove(master);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}