using Microsoft.AspNetCore.Mvc;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tasks = _context.TaskItems.OrderByDescending(t => t.CreatedAt).ToList();
            return View(tasks);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(TaskItem task)
        {
            if (ModelState.IsValid)
            {
                _context.TaskItems.Add(task);
                _context.SaveChanges();
                TempData["Toast"] = "Task created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        public IActionResult Edit(int id)
        {
            var task = _context.TaskItems.Find(id);
            return task == null ? NotFound() : View(task);
        }

        [HttpPost]
        public IActionResult Edit(TaskItem task) 
        {
            _context.TaskItems.Update(task);
            _context.SaveChanges();
            TempData["Toast"] = "Task edited successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var task = _context.TaskItems.Find(id);
            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) 
        {
            var task = _context.TaskItems.Find(id);
            _context.TaskItems.Remove(task);
            _context.SaveChanges();
            TempData["Toast"] = "Task deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ToggleComplete(int id)
        {
            var task = _context.TaskItems.Find(id);
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
