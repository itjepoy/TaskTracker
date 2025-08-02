using Microsoft.AspNetCore.Mvc;
using TaskTracker.Models;
using TaskTracker.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace TaskTracker.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View(user);
            }

            if (ModelState.IsValid)
            {

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password); // Hash the password
                _context.Users.Add(user);
                _context.SaveChanges();
                TempData["Success"] = "Registration successful!";
                return RedirectToAction("Login");
            }


            return View(user);
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                TempData["Error"] = "Invalid username or password.";
                return RedirectToAction("Login");
            }

            // Save to session
            HttpContext.Session.SetInt32("UserId", user.Id);
            TempData["Success"] = "Logged in successfully!";
            return RedirectToAction("Index", "Dashboard");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
