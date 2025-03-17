using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class AccountTypeController : Controller
    {
        private readonly ILogger<AccountTypeController> _logger;
        private readonly IAccountTypeService _accountTypeService;

        public AccountTypeController(ILogger<AccountTypeController> logger, IAccountTypeService accountTypeService)
        {
            _logger = logger;
            _accountTypeService = accountTypeService;
        }

        // GET: /master/AccountType/Index
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

                var data = await _accountTypeService.GetAccountTypeListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account types.");
                return Json(new { Result = -1, Message = "An error occurred" });
            }
        }

        // GET: /master/AccountType/GetById
        [HttpGet]
        public async Task<JsonResult> GetById(short accTypeId, string companyId)
        {
            if (accTypeId <= 0)
            {
                return Json(new { success = false, message = "Invalid Account Type ID." });
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
                var data = await _accountTypeService.GetAccountTypeByIdAsync(companyIdShort, parsedUserId, accTypeId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Account Type not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account type by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: /master/AccountType/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveAccountTypeViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var accountType = model.AccountType;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var accountTypeToSave = new M_AccountType
            {
                AccTypeId = accountType.AccTypeId,
                CompanyId = companyIdShort,
                AccTypeCode = accountType.AccTypeCode ?? string.Empty,
                AccTypeName = accountType.AccTypeName ?? string.Empty,
                SeqNo = accountType.SeqNo,
                AccGroupName = accountType.AccGroupName ?? string.Empty,
                Remarks = accountType.Remarks?.Trim() ?? string.Empty,
                IsActive = accountType.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = accountType.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _accountTypeService.SaveAccountTypeAsync(companyIdShort, parsedUserId, accountTypeToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account type." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account type.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        // DELETE: /master/AccountType/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(short accTypeId, string companyId)
        {
            if (accTypeId <= 0)
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
                var accountTypeGet = await _accountTypeService.GetAccountTypeByIdAsync(companyIdShort, parsedUserId, accTypeId);

                var data = await _accountTypeService.DeleteAccountTypeAsync(companyIdShort, 1, accountTypeGet);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account type." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account type.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }
    }
}