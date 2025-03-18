using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Controllers;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class AccountSetupController : BaseController
    {
        private readonly ILogger<AccountSetupController> _logger;
        private readonly IAccountSetupService _accountSetupService;

        public AccountSetupController(ILogger<AccountSetupController> logger,
            IBaseService baseService,
            IAccountSetupService accountSetupService)
            : base(logger, baseService)
        {
            _logger = logger;
            _accountSetupService = accountSetupService;
        }

        #region AccountSetup CRUD

        [Authorize]
        public async Task<IActionResult> Index(int? companyId)
        {
            if (!companyId.HasValue || companyId <= 0)
            {
                _logger.LogWarning("Invalid company ID: {CompanyId}", companyId);
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var parsedUserId = GetParsedUserId();
            if (!parsedUserId.HasValue)
            {
                _logger.LogWarning("User not logged in or invalid user ID.");
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var permissions = await HasPermission((short)companyId, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.AccountSetup);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _accountSetupService.GetAccountSetupListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account setup list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(short accSetupId, string companyId)
        {
            if (accSetupId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _accountSetupService.GetAccountSetupByIdAsync(companyIdShort, parsedUserId.Value, accSetupId);
                return data == null
                    ? Json(new { success = false, message = "Account Setup not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account setup by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveAccountSetupViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var setupToSave = new M_AccountSetup
                {
                    AccSetupId = model.accountSetup.AccSetupId,
                    CompanyId = companyIdShort,
                    AccSetupCode = model.accountSetup.AccSetupCode ?? string.Empty,
                    AccSetupName = model.accountSetup.AccSetupName ?? string.Empty,
                    AccSetupCategoryId = model.accountSetup.AccSetupCategoryId,
                    Remarks = model.accountSetup.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.accountSetup.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = model.accountSetup.EditById ?? 0,
                    EditDate = DateTime.UtcNow
                };

                var result = await _accountSetupService.SaveAccountSetupAsync(companyIdShort, parsedUserId.Value, setupToSave);
                return Json(new { success = true, message = "Account Setup saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving account setup");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short accSetupId, string companyId)
        {
            if (accSetupId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.AccountSetup);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var setup = await _accountSetupService.GetAccountSetupByIdAsync(companyIdShort, parsedUserId.Value, accSetupId);
                if (setup == null)
                    return Json(new { success = false, message = "Account Setup not found" });

                await _accountSetupService.DeleteAccountSetupAsync(companyIdShort, parsedUserId.Value, setup);
                return Json(new { success = true, message = "Account Setup deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account setup");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion AccountSetup CRUD

        #region AccountSetupDt CRUD

        [HttpGet]
        public async Task<JsonResult> ListDetails(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _accountSetupService.GetAccountSetupDtListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account setup details list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetDetailById(short accSetupDtId, string companyId)
        {
            if (accSetupDtId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _accountSetupService.GetAccountSetupDtByIdAsync(companyIdShort, parsedUserId.Value, accSetupDtId);
                return data == null
                    ? Json(new { success = false, message = "Account Setup Detail not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account setup detail by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveDetail([FromBody] SaveAccountSetupDtViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var detailToSave = new M_AccountSetupDt
                {
                    // AccSetupDtId = model.accountSetupDt.AccSetupDtId, // Missing in ViewModel - requires correction
                    CompanyId = companyIdShort,
                    AccSetupId = model.accountSetupDt.AccSetupId,
                    CurrencyId = model.accountSetupDt.CurrencyId,
                    GLId = model.accountSetupDt.GLId,
                    ApplyAllCurr = model.accountSetupDt.ApplyAllCurr,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = model.accountSetupDt.EditById ?? 0,
                    EditDate = DateTime.UtcNow
                };

                var result = await _accountSetupService.SaveAccountSetupDtAsync(companyIdShort, parsedUserId.Value, detailToSave);
                return Json(new { success = true, message = "Account Setup Detail saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving account setup detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDetail(short accSetupDtId, string companyId)
        {
            if (accSetupDtId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.AccountSetup);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var detail = await _accountSetupService.GetAccountSetupDtByIdAsync(companyIdShort, parsedUserId.Value, accSetupDtId);
                if (detail == null)
                    return Json(new { success = false, message = "Account Setup Detail not found" });

                await _accountSetupService.DeleteAccountSetupDtAsync(companyIdShort, parsedUserId.Value, detail);
                return Json(new { success = true, message = "Account Setup Detail deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account setup detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion AccountSetupDt CRUD

        #region AccountSetupCategory CRUD

        [Authorize]
        public async Task<IActionResult> CategoryIndex(int? companyId)
        {
            if (!companyId.HasValue || companyId <= 0)
            {
                _logger.LogWarning("Invalid company ID: {CompanyId}", companyId);
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var parsedUserId = GetParsedUserId();
            if (!parsedUserId.HasValue)
            {
                _logger.LogWarning("User not logged in or invalid user ID.");
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var permissions = await HasPermission((short)companyId, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.AccountSetupCategory);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> ListCategories(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _accountSetupService.GetAccountSetupCategoryListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account setup category list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCategoryById(short accSetupCategoryId, string companyId)
        {
            if (accSetupCategoryId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _accountSetupService.GetAccountSetupCategoryByIdAsync(companyIdShort, parsedUserId.Value, accSetupCategoryId);
                return data == null
                    ? Json(new { success = false, message = "Account Setup Category not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account setup category by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCategory([FromBody] SaveAccountSetupCategoryViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var categoryToSave = new M_AccountSetupCategory
                {
                    AccSetupCategoryId = model.accountSetupCategory.AccSetupCategoryId,
                    AccSetupCategoryCode = model.accountSetupCategory.AccSetupCategoryCode ?? string.Empty,
                    AccSetupCategoryName = model.accountSetupCategory.AccSetupCategoryName ?? string.Empty,
                    Remarks = model.accountSetupCategory.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.accountSetupCategory.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = model.accountSetupCategory.EditById ?? 0,
                    EditDate = DateTime.UtcNow
                };

                var result = await _accountSetupService.SaveAccountSetupCategoryAsync(companyIdShort, parsedUserId.Value, categoryToSave);
                return Json(new { success = true, message = "Account Setup Category saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving account setup category");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(short accSetupCategoryId, string companyId)
        {
            if (accSetupCategoryId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.AccountSetupCategory);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var category = await _accountSetupService.GetAccountSetupCategoryByIdAsync(companyIdShort, parsedUserId.Value, accSetupCategoryId);
                if (category == null)
                    return Json(new { success = false, message = "Account Setup Category not found" });

                await _accountSetupService.DeleteAccountSetupCategoryAsync(companyIdShort, parsedUserId.Value, category);
                return Json(new { success = true, message = "Account Setup Category deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account setup category");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion AccountSetupCategory CRUD
    }
}