using AEMSWEB.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Controllers
{
    [Authorize]
    [Route("{companyId:int}/[controller]")]
    public class DashboardController : BaseController
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly Random _random = new Random();

        public DashboardController(ILogger<DashboardController> logger, IBaseService baseService)
            : base(logger, baseService)
        {
            _logger = logger;
        }

        private dynamic GenerateDummyData(string dashboardType, int companyId)
        {
            // Seed random with companyId for consistent dummy data per company
            var seededRandom = new Random(companyId);

            return dashboardType switch
            {
                "Finance" => new
                {
                    Revenue = seededRandom.Next(50000, 200000) + companyId * 1000,
                    Expenses = seededRandom.Next(30000, 150000) + companyId * 500,
                    Profit = seededRandom.Next(20000, 100000) + companyId * 300,
                    PendingInvoices = seededRandom.Next(1, 15)
                },
                "Hrm" => new
                {
                    // Existing HRM data
                    TotalEmployees = seededRandom.Next(50, 200),
                    PresentToday = seededRandom.Next(40, 180),
                    PendingLeaves = seededRandom.Next(1, 10),
                    UpcomingPayroll = seededRandom.Next(50000, 100000) + companyId * 1000,

                    // New employee-focused data
                    AvailableLeaves = seededRandom.Next(10, 30),
                    UsedLeaves = seededRandom.Next(0, 10),
                    AttendancePercentage = seededRandom.Next(80, 100),
                    WorkingDays = 22,
                    PresentDays = seededRandom.Next(18, 22),
                    ActiveLoans = seededRandom.Next(0, 3),
                    LoanBalance = seededRandom.Next(0, 5000),
                    NetSalary = seededRandom.Next(3000, 10000),

                    // Employee details
                    EmployeeName = "John Doe",
                    EmployeeId = $"EMP{companyId:000}",
                    EmployeeDesignation = "Senior Developer",

                    // Salary breakdown
                    SalaryEarnings = new Dictionary<string, decimal>
                    {
                        ["Basic Salary"] = 7500,
                        ["Housing Allowance"] = 1500,
                        ["Transport Allowance"] = 500
                    },
                    SalaryDeductions = new Dictionary<string, decimal>
                    {
                        ["Income Tax"] = 1200,
                        ["Health Insurance"] = 300,
                        ["Loan Deduction"] = 500
                    },

                    // Leave calendar data (fixed end date format)
                    LeaveEvents = new[]
            {
                new {
                    title = "Annual Leave",
                    start = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd"),
                    end = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd"),
                    color = "#4CAF50"
                },
                new {
                    title = "Sick Leave",
                    start = DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd"),
                    end = DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd"),  // Single day event
                    color = "#F44336"
                }
            },

                    // Loan details (fixed comma separation)
                    ActiveLoanDetails = new[]
            {
                new {
                    LoanType = "Home Loan",
                    Date = DateTime.Now.AddMonths(-6),
                    RemainingAmount = 2500m,
                    Status = "Active"
                },
                new {
                    LoanType = "Vehicle Loan",
                    Date = DateTime.Now.AddMonths(-2),
                    RemainingAmount = 1500m,
                    Status = "Active"
                }
            }
                },
                "Job" => new
                {
                    OpenPositions = seededRandom.Next(5, 30) + companyId,
                    TotalApplications = seededRandom.Next(50, 300) + companyId * 5,
                    InterviewsScheduled = seededRandom.Next(5, 20) + companyId
                },
                "Account" => new
                {
                    // Existing account data
                    TotalAccounts = seededRandom.Next(100, 500) + companyId * 10,
                    ActiveSessions = seededRandom.Next(50, 200) + companyId * 2,
                    PendingVerifications = seededRandom.Next(1, 15),

                    // New financial metrics
                    TotalInvoices = seededRandom.Next(50, 200),
                    InvoiceValueAED = seededRandom.Next(100000, 500000),
                    InvoiceValueUSD = seededRandom.Next(100000, 500000) / 3.67m,
                    TotalReceipt = seededRandom.Next(50000, 200000),
                    TotalPayable = seededRandom.Next(50000, 150000),
                    TotalReceivable = seededRandom.Next(50000, 200000),

                    // Bank balances
                    BankBalances = new[] {
                        new {
                            BankName = "Emirates NBD",
                            AccountNumber = "**** " + seededRandom.Next(1000,9999),
                            Balance = seededRandom.Next(100000, 500000)
                        },
                        new {
                            BankName = "Mashreq Bank",
                            AccountNumber = "**** " + seededRandom.Next(1000,9999),
                            Balance = seededRandom.Next(50000, 200000)
                        }
                    },

                    // Trial balance
                    TrialBalance = new
                    {
                        Debit = seededRandom.Next(100000, 500000),
                        Credit = seededRandom.Next(100000, 500000),
                        IsBalanced = seededRandom.NextDouble() > 0.5
                    },

                    // Cash flow data
                    CashFlowLabels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                    CashFlowData = new[] {
                        seededRandom.Next(-50000, 100000),
                        seededRandom.Next(-50000, 100000),
                        seededRandom.Next(-50000, 100000),
                        seededRandom.Next(-50000, 100000),
                        seededRandom.Next(-50000, 100000),
                        seededRandom.Next(-50000, 100000)
                    },

                    RecentTransactions = new[] {
                        new {
                            Description = "Invoice Payment",
                            Amount = seededRandom.Next(1000, 5000),
                            Date = DateTime.Now.AddDays(-1),
                            Type = "Credit"
                        },
                        new {
                            Description = "Vendor Payment",
                            Amount = seededRandom.Next(1000, 5000),
                            Date = DateTime.Now.AddDays(-2),
                            Type = "Debit"
                        }
                    }
                },
                _ => new
                {
                    TotalCompanies = seededRandom.Next(10, 100) + companyId,
                    ActiveUsers = seededRandom.Next(50, 500) + companyId * 5,
                    RecentActivities = seededRandom.Next(5, 20)
                }
            };
        }

        [HttpGet("Index")]
        public IActionResult Index(int companyId)
        {
            var data = GenerateDummyData("Main", companyId);
            ViewBag.CompanyId = companyId;
            ViewBag.TotalCompanies = data.TotalCompanies;
            ViewBag.ActiveUsers = data.ActiveUsers;
            ViewBag.RecentActivities = data.RecentActivities;
            return View();
        }

        [HttpGet("Finance")]
        public IActionResult Finance(int companyId)
        {
            var data = GenerateDummyData("Finance", companyId);
            ViewBag.CompanyId = companyId;
            ViewBag.Revenue = data.Revenue;
            ViewBag.Expenses = data.Expenses;
            ViewBag.Profit = data.Profit;
            ViewBag.PendingInvoices = data.PendingInvoices;
            return View();
        }

        [HttpGet("Hrm")]
        public async Task<IActionResult> Hrm(int companyId)
        {
            var data = GenerateDummyData("Hrm", companyId);
            ViewBag.CompanyId = companyId;
            ViewBag.TotalEmployees = data.TotalEmployees;
            ViewBag.PresentToday = data.PresentToday;
            ViewBag.PendingLeaves = data.PendingLeaves;
            ViewBag.UpcomingPayroll = data.UpcomingPayroll;

            // New ViewBag assignments for additional data
            ViewBag.AvailableLeaves = data.AvailableLeaves;
            ViewBag.UsedLeaves = data.UsedLeaves;
            ViewBag.AttendancePercentage = data.AttendancePercentage;
            ViewBag.WorkingDays = data.WorkingDays;
            ViewBag.PresentDays = data.PresentDays;
            ViewBag.ActiveLoans = data.ActiveLoans;
            ViewBag.LoanBalance = data.LoanBalance;
            ViewBag.NetSalary = data.NetSalary;
            ViewBag.EmployeeName = data.EmployeeName;
            ViewBag.EmployeeId = data.EmployeeId;
            ViewBag.EmployeeDesignation = data.EmployeeDesignation;
            ViewBag.SalaryEarnings = data.SalaryEarnings;
            ViewBag.SalaryDeductions = data.SalaryDeductions;
            ViewBag.LeaveEvents = data.LeaveEvents;
            ViewBag.ActiveLoanDetails = data.ActiveLoanDetails;

            return View();
        }

        [HttpGet("Job")]
        public IActionResult Job(int companyId)
        {
            var data = GenerateDummyData("Job", companyId);
            ViewBag.CompanyId = companyId;
            ViewBag.OpenPositions = data.OpenPositions;
            ViewBag.TotalApplications = data.TotalApplications;
            ViewBag.InterviewsScheduled = data.InterviewsScheduled;
            return View();
        }

        [HttpGet("Account")]
        public IActionResult Account(int companyId)
        {
            var data = GenerateDummyData("Account", companyId);

            ViewBag.CompanyId = companyId;

            // Account metrics
            ViewBag.TotalAccounts = data.TotalAccounts;
            ViewBag.ActiveSessions = data.ActiveSessions;
            ViewBag.PendingVerifications = data.PendingVerifications;

            // Financial metrics
            ViewBag.TotalInvoices = data.TotalInvoices;
            ViewBag.InvoiceValueAED = data.InvoiceValueAED;
            ViewBag.InvoiceValueUSD = data.InvoiceValueUSD;
            ViewBag.TotalReceipt = data.TotalReceipt;
            ViewBag.TotalPayable = data.TotalPayable;
            ViewBag.TotalReceivable = data.TotalReceivable;

            // Bank data
            ViewBag.BankBalances = data.BankBalances;
            ViewBag.TrialBalance = data.TrialBalance;

            // Chart data
            ViewBag.CashFlowLabels = data.CashFlowLabels;
            ViewBag.CashFlowData = data.CashFlowData;
            ViewBag.RecentTransactions = data.RecentTransactions;

            return View();
        }

        [HttpGet("Documents")]
        public IActionResult Documents(int companyId)
        {
            var data = GenerateDummyData("Job", companyId);
            ViewBag.CompanyId = companyId;
            ViewBag.OpenPositions = data.OpenPositions;
            ViewBag.TotalApplications = data.TotalApplications;
            ViewBag.InterviewsScheduled = data.InterviewsScheduled;
            return View();
        }
    }
}