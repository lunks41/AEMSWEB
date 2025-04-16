namespace AMESWEB.Areas.HRM.Models
{
    public class DashboardViewModel
    {
        public Employee Employee { get; set; }
        public List<Attendance> Attendances { get; set; }
        public List<LeaveBalance> LeaveBalances { get; set; }
        public List<Payroll> Payrolls { get; set; }
        public List<Leave> UpcomingLeaves { get; set; }
    }
}