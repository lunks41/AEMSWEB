using AEMSWEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.PettyCash.Controllers
{
    [Authorize]
    public class PettyCashController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;

        public PettyCashController(ApplicationDbContext context,
                                 IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        //// GET: PettyCash
        //public async Task<IActionResult> Index()
        //{
        //    var entries = await _context.PettyCashEntries
        //        .OrderByDescending(e => e.TransactionDate)
        //        .ToListAsync();
        //    return View(entries);
        //}

        //// GET: PettyCash/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(PettyCashEntry entry)
        //{
        //    entry.RequestedBy = User.Identity.Name;
        //    entry.Status = ApprovalStatus.Pending;

        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(entry);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(entry);
        //}

        //// GET: PettyCash/Upload
        //public IActionResult Upload()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Upload(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        ModelState.AddModelError("", "Please select a valid Excel file");
        //        return View();
        //    }

        //    var entries = new List<PettyCashEntry>();
        //    var errors = new List<string>();

        //    using (var stream = new MemoryStream())
        //    {
        //        await file.CopyToAsync(stream);

        //        using (var package = new ExcelPackage(stream))
        //        {
        //            var worksheet = package.Workbook.Worksheets[0];
        //            var rowCount = worksheet.Dimension.Rows;

        //            for (int row = 2; row <= rowCount; row++) // Skip header row
        //            {
        //                try
        //                {
        //                    var entry = new PettyCashEntry
        //                    {
        //                        TransactionDate = DateTime.Parse(worksheet.Cells[row, 1].Text),
        //                        Description = worksheet.Cells[row, 2].Text,
        //                        Amount = decimal.Parse(worksheet.Cells[row, 3].Text),
        //                        Category = worksheet.Cells[row, 4].Text,
        //                        EmployeeId = worksheet.Cells[row, 5].Text,
        //                        ApprovedBy = worksheet.Cells[row, 6].Text
        //                    };

        //                    // Validate model
        //                    var validationResults = new List<ValidationResult>();
        //                    var isValid = Validator.TryValidateObject(entry,
        //                        new ValidationContext(entry),
        //                        validationResults,
        //                        true);

        //                    if (isValid)
        //                    {
        //                        entries.Add(entry);
        //                    }
        //                    else
        //                    {
        //                        errors.Add($"Row {row}: {string.Join(", ", validationResults.Select(v => v.ErrorMessage))}");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    errors.Add($"Row {row}: Error processing - {ex.Message}");
        //                }
        //            }
        //        }
        //    }

        //    if (entries.Any())
        //    {
        //        await _context.PettyCashEntries.AddRangeAsync(entries);
        //        await _context.SaveChangesAsync();
        //    }

        //    // Add error handling view or display
        //    TempData["Message"] = $"Successfully imported {entries.Count} entries";
        //    TempData["Errors"] = errors;
        //    return RedirectToAction("Results");
        //}

        //public IActionResult Results()
        //{
        //    return View();
        //}

        //[Authorize(Roles = "Manager")]
        //public async Task<IActionResult> Approve(int id)
        //{
        //    var entry = await _context.PettyCashEntries.FindAsync(id);
        //    if (entry == null) return NotFound();

        //    entry.Status = ApprovalStatus.Approved;
        //    entry.ApprovedBy = User.Identity.Name;
        //    entry.ApprovalDate = DateTime.Now;

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //[Authorize(Roles = "Manager")]
        //public async Task<IActionResult> Reject(int id, string comments)
        //{
        //    var entry = await _context.PettyCashEntries.FindAsync(id);
        //    if (entry == null) return NotFound();

        //    entry.Status = ApprovalStatus.Rejected;
        //    entry.ApprovedBy = User.Identity.Name;
        //    entry.ApprovalDate = DateTime.Now;
        //    entry.Comments = comments;

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        //public IActionResult DownloadTemplate()
        //{
        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Template");

        //        // Create header row
        //        worksheet.Cells[1, 1].Value = "Date (yyyy-mm-dd)";
        //        worksheet.Cells[1, 2].Value = "Description";
        //        worksheet.Cells[1, 3].Value = "Amount";
        //        worksheet.Cells[1, 4].Value = "Category";
        //        worksheet.Cells[1, 5].Value = "Employee ID";

        //        // Add data validation
        //        var categoryList = new[] { "Office Supplies", "Travel", "Meals", "Utilities" };
        //        var validation = worksheet.DataValidations.AddListValidation("D2:D1000");
        //        validation.ShowErrorMessage = true;
        //        validation.ErrorTitle = "Invalid Category";
        //        validation.Error = "Select from available categories";
        //        foreach (var category in categoryList)
        //        {
        //            validation.Formula.Values.Add(category);
        //        }

        //        var stream = new MemoryStream();
        //        package.SaveAs(stream);
        //        stream.Position = 0;

        //        return File(stream,
        //                 "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                 "petty-cash-template.xlsx");
        //    }
        //}
    }
}
