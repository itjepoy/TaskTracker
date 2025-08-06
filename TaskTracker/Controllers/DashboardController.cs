using Microsoft.AspNetCore.Mvc;
using TaskTracker.Data;

namespace TaskTracker.Controllers
{
    public class DashboardController : Controller
    {
        public readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to access this page.";
                return RedirectToAction("Login", "User");
            }

            var userTasks = _context.Tasks.Where(t => t.UserId == userId);

            var totalTasks = userTasks.Count();
            var completedTasks = userTasks.Count(t => t.IsCompleted);
            var incompleteTasks = totalTasks - completedTasks;

            ViewData["Total"] = totalTasks;
            ViewData["Completed"] = completedTasks;
            ViewData["Incomplete"] = incompleteTasks;

            return View();
        }

    }
}
