using Microsoft.AspNetCore.Mvc;
using AutoPartsPOS.Data;
using AutoPartsPOS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AutoPartsPOS.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var hash = HashPassword(model.Password);

            var user = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefault(u => u.Username == model.Username && u.PasswordHash == hash);

            if (user == null)
            {
                ViewBag.Error = "بيانات الدخول غير صحيحة";
                return View(model);
            }
HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            HttpContext.Session.SetString("Roles", string.Join(",", roles));

            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct();
            HttpContext.Session.SetString("Permissions", string.Join(",", permissions));

            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
