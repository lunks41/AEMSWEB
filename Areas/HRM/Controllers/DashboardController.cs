using AEMSWEB.Areas.HRM.Models;
using AEMSWEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AEMSWEB.Areas.HRM.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefault(e => e.UserId == userId);

            if (employee == null) return RedirectToAction("Index");

            var model = new DashboardViewModel
            {
                Employee = employee,
                Attendances = _context.Attendances.Where(a => a.EmployeeId == employee.Id).Take(10).ToList(),
                LeaveBalances = _context.LeaveBalances.Include(lb => lb.LeaveType).Where(lb => lb.EmployeeId == employee.Id).ToList(),
                Payrolls = _context.Payrolls.Where(p => p.EmployeeId == employee.Id).Take(5).ToList(),
                UpcomingLeaves = _context.Leaves.Where(l => l.EmployeeId == employee.Id && l.StartDate >= DateTime.Now && l.Status == "Approved").ToList()
            };

            return View();
        }
    }
}
