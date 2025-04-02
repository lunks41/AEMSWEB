using AEMSWEB.Areas.HRM.Models;
using AEMSWEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.HRM.Controllers
{
    [Authorize(Roles = "HR,Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Employees
            //    .Include(e => e.Department)
            //    .ToListAsync());

            return View();
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            //ViewBag.Departments = _context.Departments.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // Add Edit, Details, Delete actions
    }
}
