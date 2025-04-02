using AEMSWEB.Areas.HRM.Models;
using AEMSWEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.HRM.Controllers
{
    public class LeaveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Employee,Manager")]
        public IActionResult RequestLeave()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> RequestLeave(LeaveRequest request)
        {
            //if (ModelState.IsValid)
            //{
            //    request.EmployeeId = GetCurrentEmployeeId();
            //    _context.Add(request);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(MyLeaves));
            //}
            //return View(request);

            return View();
        }

        [Authorize(Roles = "Manager,HR")]
        public async Task<IActionResult> PendingApprovals()
        {
            //var pending = await _context.LeaveRequests
            //    .Include(l => l.Employee)
            //    .Where(l => l.Status == LeaveStatus.Pending)
            //    .ToListAsync();

            //return View(pending);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Manager,HR")]
        public async Task<IActionResult> ApproveLeave(int id)
        {
            //var leave = await _context.LeaveRequests.FindAsync(id);
            //if (leave == null) return NotFound();

            //leave.Status = LeaveStatus.Approved;
            //leave.ApproverComments = "Approved by " + User.Identity.Name;
            //await _context.SaveChangesAsync();

            //return RedirectToAction(nameof(PendingApprovals));
            return View();
        }
    }
}
