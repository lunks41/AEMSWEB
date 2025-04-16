using AMESWEB.Areas.HRM.Models;
using AMESWEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AMESWEB.Areas.HRM.Controllers
{
    [Authorize(Roles = "HR,Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var employees = _context.Employees.Include(e => e.Department).Include(e => e.Position).ToList();
            return View(employees);
        }

        public IActionResult Create()
        {
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name");
            ViewBag.Positions = new SelectList(_context.Positions, "Id", "Title");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name");
            ViewBag.Positions = new SelectList(_context.Positions, "Id", "Title");
            return View(employee);
        }

        // Add Edit and Delete actions similarly
    }
}