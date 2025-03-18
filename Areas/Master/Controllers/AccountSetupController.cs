using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Areas.Master.Data.Services;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class AccountSetupController : Controller
    {
        private readonly ILogger<AccountSetupController> _logger;
        private readonly IAccountSetupService _accountSetupService;

        public AccountSetupController(ILogger<AccountSetupController> logger, IAccountSetupService accountSetupService)
        {
            _logger = logger;
            _accountSetupService = accountSetupService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Account Setup

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

                var data = await _accountSetupService.GetAccountSetupListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account setups.");
                return Json(new { Result = -1, Message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(short accSetupId, string companyId)
        {
            if (accSetupId <= 0)
            {
                return Json(new { success = false, message = "Invalid Account Setup ID." });
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
                var data = await _accountSetupService.GetAccountSetupByIdAsync(companyIdShort, parsedUserId, accSetupId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Account Setup not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account setup by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveAccountSetupViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var accountSetup = model.AccountSetup;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var accountSetupToSave = new M_AccountSetup
            {
                AccSetupId = accountSetup.AccSetupId,
                CompanyId = companyIdShort,
                AccSetupCode = accountSetup.AccSetupCode ?? string.Empty,
                AccSetupName = accountSetup.AccSetupName ?? string.Empty,
                AccSetupCategoryId = accountSetup.AccSetupCategoryId,
                Remarks = accountSetup.Remarks?.Trim() ?? string.Empty,
                IsActive = accountSetup.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = accountSetup.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _accountSetupService.SaveAccountSetupAsync(companyIdShort, parsedUserId, accountSetupToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account setup." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account setup.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short accSetupId, string companyId)
        {
            if (accSetupId <= 0)
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
                var accountSetupGet = await _accountSetupService.GetAccountSetupByIdAsync(companyIdShort, parsedUserId, accSetupId);

                var data = await _accountSetupService.DeleteAccountSetupAsync(companyIdShort, 1, accountSetupGet);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account setup." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account setup.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Account Setup

        #region Account Setup Category

        [HttpGet]
        public async Task<JsonResult> AccountSetupCategoryList(short pageNumber, short pageSize, string searchString, string companyId)
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

                var data = await _accountSetupService.GetAccountSetupCategoryListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account setup categories.");
                return Json(new { Result = -1, Message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> AccountSetupCategoryGetById(short accSetupCategoryId, string companyId)
        {
            if (accSetupCategoryId <= 0)
            {
                return Json(new { success = false, message = "Invalid Account Setup Category ID." });
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
                var data = await _accountSetupService.GetAccountSetupCategoryByIdAsync(companyIdShort, parsedUserId, accSetupCategoryId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Account Setup Category not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account setup category by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AccountSetupCategorySave([FromBody] SaveAccountSetupCategoryViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var accountSetupCategory = model.AccountSetupCategory;
            var companyId = model.CompanyId;

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var accountSetupCategoryToSave = new M_AccountSetupCategory
            {
                AccSetupCategoryId = accountSetupCategory.AccSetupCategoryId,
                AccSetupCategoryCode = accountSetupCategory.AccSetupCategoryCode ?? string.Empty,
                AccSetupCategoryName = accountSetupCategory.AccSetupCategoryName ?? string.Empty,
                Remarks = accountSetupCategory.Remarks?.Trim() ?? string.Empty,
                IsActive = accountSetupCategory.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = accountSetupCategory.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _accountSetupService.SaveAccountSetupCategoryAsync(companyIdShort, parsedUserId, accountSetupCategoryToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account setup category." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account setup category.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> AccountSetupCategoryDelete(short accSetupCategoryId, string companyId)
        {
            if (accSetupCategoryId <= 0)
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
                var accountSetupCategoryGet = await _accountSetupService.GetAccountSetupCategoryByIdAsync(companyIdShort, parsedUserId, accSetupCategoryId);

                var data = await _accountSetupService.DeleteAccountSetupCategoryAsync(companyIdShort, 1, accountSetupCategoryGet);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account setup category." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account setup category.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Account Setup Category
    }
}