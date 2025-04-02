using AEMSWEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            //var vm = new DashboardViewModel
            //{
            //    TotalEmployees = _context.Employees.Count(),
            //    OnLeave = _context.LeaveRequests
            //        .Count(l => l.Status == LeaveStatus.Approved &&
            //                  l.EndDate >= DateTime.Today),
            //    PendingRequests = _context.LeaveRequests
            //        .Count(l => l.Status == LeaveStatus.Pending),
            //    RecentHires = _context.Employees
            //        .OrderByDescending(e => e.HireDate)
            //        .Take(5)
            //        .ToList()
            //};

            return View();
        }
    }
}
