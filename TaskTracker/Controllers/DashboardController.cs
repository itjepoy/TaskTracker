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

            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["Error"] = "You must be logged in to access this page.";
                return RedirectToAction("Login", "User");
            }

            var totalTasks = _context.TaskItems.Count();
            var completedTasks = _context.TaskItems.Count(t => t.IsCompleted);
            var incompleteTasks = totalTasks - completedTasks;

            ViewData["Total"] = totalTasks;
            ViewData["Completed"] = completedTasks;
            ViewData["Incomplete"] = incompleteTasks;

            return View();
        }
    }
}
