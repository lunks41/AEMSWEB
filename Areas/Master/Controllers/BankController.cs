using AEMSWEB.Controllers;
using AEMSWEB.Areas.Master.Models;
using AEMSWEB.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using AEMSWEB.Areas.Master.Data.IServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AEMSWEB.Entities.Masters;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class BankController : Controller
    {
        private readonly ILogger<BankController> _logger;
        private readonly IBankService _bankService;

        public BankController(ILogger<BankController> logger, IBankService bankService)
        {
            _logger = logger;
            _bankService = bankService;
        }

        // GET: /master/Bank/Index
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("List")]
        public async Task<JsonResult> List(short pageNumber, short pageSize, string searchString, string companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
                {
                    return Json(new { Result = -1, Message = "Invalid company ID" });
                }

                var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var data = await _bankService.GetBankListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching banks.");
                return Json(new { Result = -1, Message = "An error occurred" });
            }
        }

        // GET: /master/Bank/GetById
        [HttpGet]
        public async Task<JsonResult> GetById(short bankId, string companyId)
        {
            if (bankId <= 0)
            {
                return Json(new { success = false, message = "Invalid Bank ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                var data = await _bankService.GetBankByIdAsync(companyIdShort, parsedUserId, bankId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Bank not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching bank by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: /master/Bank/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveBankViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var bank = model.Bank;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var bankToSave = new M_Bank
            {
                BankId = bank.BankId,
                CompanyId = companyIdShort,
                BankCode = bank.BankCode ?? string.Empty,
                BankName = bank.BankName ?? string.Empty,
                CurrencyId = bank.CurrencyId,
                AccountNo = bank.AccountNo ?? string.Empty,
                SwiftCode = bank.SwiftCode ?? string.Empty,
                Remarks1 = bank.Remarks1?.Trim() ?? string.Empty,
                Remarks2 = bank.Remarks2?.Trim() ?? string.Empty,
                GLId = bank.GLId,
                IsActive = bank.IsActive,
                IsOwnBank = bank.IsOwnBank,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = bank.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _bankService.SaveBankAsync(companyIdShort, parsedUserId, bankToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save bank." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the bank.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        // DELETE: /master/Bank/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(short bankId, string companyId)
        {
            if (bankId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                var data = await _bankService.DeleteBankAsync(companyIdShort, 1, bankId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save bank." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the bank.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }
    }
}