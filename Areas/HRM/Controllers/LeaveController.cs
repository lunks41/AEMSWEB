using AMESWEB.Areas.HRM.Models;
using AMESWEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AMESWEB.Areas.HRM.Controllers
{
    [Authorize]
    public class LeaveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Apply()
        {
            ViewBag.LeaveTypes = new SelectList(_context.LeaveTypes, "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Apply(Leave leave)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.Employees.FirstOrDefault(e => e.UserId == userId);
            if (ModelState.IsValid && employee != null)
            {
                leave.EmployeeId = employee.Id;
                leave.Days = (leave.EndDate - leave.StartDate).Days + 1;
                _context.Leaves.Add(leave);
                _context.SaveChanges();
                TempData["Message"] = "Leave applied successfully.";
                return RedirectToAction("Dashboard", "Home");
            }
            ViewBag.LeaveTypes = new SelectList(_context.LeaveTypes, "Id", "Name");
            return View(leave);
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Pending()
        {
            var leaves = _context.Leaves.Where(l => l.Status == "Pending").Include(l => l.Employee).Include(l => l.LeaveType).ToList();
            return View(leaves);
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Approve(int id)
        {
            var leave = _context.Leaves.Find(id);
            if (leave != null)
            {
                leave.Status = "Approved";
                leave.ApprovedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var balance = _context.LeaveBalances.FirstOrDefault(lb => lb.EmployeeId == leave.EmployeeId && lb.LeaveTypeId == leave.LeaveTypeId);
                if (balance != null && balance.Balance >= leave.Days)
                {
                    balance.Balance -= leave.Days;
                    _context.SaveChanges();
                    TempData["Message"] = "Leave approved.";
                }
            }
            return RedirectToAction("Pending");
        }

        // Add Reject action similarly
    }
}