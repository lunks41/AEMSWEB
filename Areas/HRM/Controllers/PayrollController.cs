using AEMSWEB.Areas.HRM.Models;
using AEMSWEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AEMSWEB.Areas.HRM.Controllers
{
    [Authorize(Roles = "HR")]
    public class PayrollController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayrollController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var payrolls = _context.Payrolls.Include(p => p.Employee).ToList();
            return View(payrolls);
        }

        public IActionResult Generate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generate(DateTime payPeriodStart, DateTime payPeriodEnd)
        {
            var employees = _context.Employees.ToList();
            foreach (var employee in employees)
            {
                var payroll = new Payroll
                {
                    EmployeeId = employee.Id,
                    PayPeriodStart = payPeriodStart,
                    PayPeriodEnd = payPeriodEnd,
                    GrossSalary = employee.BaseSalary,
                    Deductions = 500, // Fixed for simplicity
                    NetSalary = employee.BaseSalary - 500
                };
                _context.Payrolls.Add(payroll);
            }
            _context.SaveChanges();
            TempData["Message"] = "Payroll generated successfully.";
            return RedirectToAction("Index");
        }
    }
}
