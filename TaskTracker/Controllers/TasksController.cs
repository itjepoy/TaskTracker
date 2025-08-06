using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Controllers
{
    public class TasksController : Controller
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["Error"] = "You must be logged in to access this page.";
                context.Result = RedirectToAction("Login", "User");
            }
            base.OnActionExecuting(context);
        }

        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to view tasks.";
                return RedirectToAction("Login", "User");
            }

            var query = _context.Tasks
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(search))
            {
                // Case-insensitive search using EF.Functions.Like (better for SQL)
                query = query.Where(t => EF.Functions.Like(t.Title, $"%{search}%"));
            }

            var userTasks = query.ToList();

            return View(userTasks);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskItem task)
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to create a task.";
                return RedirectToAction("Login", "User");
            }

            task.UserId = userId.Value;

            if (ModelState.IsValid)
            {
                _context.Add(task);
                _context.SaveChanges();
                TempData["Toast"] = "Task created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        public IActionResult Edit(int id)
        {
            var task = _context.Tasks.Find(id);
            return task == null ? NotFound() : View(task);
        }

        [HttpPost]
        public IActionResult Edit(TaskItem task)
        {
            var existingTask = _context.Tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existingTask == null)
                return NotFound();

            // Only update allowed fields
            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.IsCompleted = task.IsCompleted;

            _context.SaveChanges();

            TempData["Toast"] = "Task edited successfully.";
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            var task = _context.Tasks.Find(id);
            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) 
        {
            var task = _context.Tasks.Find(id);
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            TempData["Toast"] = "Task deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ToggleComplete(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
