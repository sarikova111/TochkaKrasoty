using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TochkaKrasoty.Data;
using System.Linq;

namespace TochkaKrasoty.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SalonDbContext _context;

        public LoginModel(SalonDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Input.Email) || string.IsNullOrWhiteSpace(Input.Password))
            {
                ModelState.AddModelError(string.Empty, "Введите email и пароль");
                return Page();
            }

            var email = Input.Email.Trim().ToLower();
            var password = Input.Password.Trim();

            var user = _context.Users.FirstOrDefault(u =>
                u.Email.ToLower() == email && u.Password == password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неверный email или пароль");
                return Page();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.FullName);

            if (user.Role == "Admin")
            {
                return RedirectToPage("/Admin/Dashboard");
            }

            return RedirectToPage("/Profile");
        }
    }
}