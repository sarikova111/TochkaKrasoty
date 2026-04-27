using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TochkaKrasoty.Data;
using TochkaKrasoty.Models;
using System.Linq;

namespace TochkaKrasoty.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly SalonDbContext _context;

        public RegisterModel(SalonDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public string FullName { get; set; } = "";
            public string Email { get; set; } = "";
            public string? Phone { get; set; }
            public string Password { get; set; } = "";
            public string ConfirmPassword { get; set; } = "";
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Input.FullName) ||
                string.IsNullOrWhiteSpace(Input.Email) ||
                string.IsNullOrWhiteSpace(Input.Password) ||
                string.IsNullOrWhiteSpace(Input.ConfirmPassword))
            {
                ModelState.AddModelError(string.Empty, "Заполните все поля");
                return Page();
            }

            if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Пароли не совпадают");
                return Page();
            }

            var email = Input.Email.Trim().ToLower();
            var password = Input.Password.Trim();

            var existingUser = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь с таким email уже существует");
                return Page();
            }

            var user = new User
            {
                FullName = Input.FullName.Trim(),
                Email = email,
                Password = password,
                Phone = string.IsNullOrWhiteSpace(Input.Phone) ? null : Input.Phone.Trim(),
                Role = "Client"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToPage("/Profile");
        }
    }
}