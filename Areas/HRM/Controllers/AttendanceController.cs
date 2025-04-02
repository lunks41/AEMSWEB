using AEMSWEB.Areas.HRM.Models;
using AEMSWEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace AEMSWEB.Areas.HRM.Controllers
{
    [Authorize(Roles = "HR")]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var attendances = _context.Attendances.Include(a => a.Employee).ToList();
            return View(attendances);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var employeeCode = worksheet.Cells[row, 1].Value?.ToString();
                        var date = DateTime.Parse(worksheet.Cells[row, 2].Value.ToString());
                        var inTime = TimeSpan.Parse(worksheet.Cells[row, 3].Value.ToString());
                        var outTime = TimeSpan.Parse(worksheet.Cells[row, 4].Value.ToString());

                        var employee = _context.Employees.FirstOrDefault(e => e.EmployeeCode == employeeCode);
                        if (employee != null)
                        {
                            var attendance = new Attendance
                            {
                                EmployeeId = employee.Id,
                                Date = date,
                                InTime = inTime,
                                OutTime = outTime,
                                Status = "Present" // Can enhance later
                            };
                            _context.Attendances.Add(attendance);
                        }
                    }
                    _context.SaveChanges();
                    TempData["Message"] = "Attendance uploaded successfully.";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
