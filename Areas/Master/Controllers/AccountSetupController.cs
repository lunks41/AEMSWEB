using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Controllers;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        #region AccountSetup CRUD

        [HttpGet]
        public async Task<JsonResult> AccountSetupList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
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
        public async Task<JsonResult> GetAccountSetupById(short accSetupId, string companyId)
        {
            if (accSetupId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
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
        public async Task<IActionResult> SaveAccountSetup([FromBody] SaveAccountSetupViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
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
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
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
        public async Task<IActionResult> DeleteAccountSetup(short accSetupId, string companyId)
        {
            if (accSetupId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.AccountSetup);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _accountSetupService.DeleteAccountSetupAsync(companyIdShort, parsedUserId.Value, accSetupId);
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
        public async Task<JsonResult> AccountSetupDtList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
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
        public async Task<JsonResult> GetAccountSetupDtById(short accSetupId, short currencyId, short glId, string companyId)
        {
            if (accSetupId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _accountSetupService.GetAccountSetupDtByIdAsync(companyIdShort, parsedUserId.Value, accSetupId, currencyId, glId);
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
        public async Task<IActionResult> SaveAccountSetupDt([FromBody] SaveAccountSetupDtViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
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
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
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
        public async Task<IActionResult> DeleteAccountSetupDt(short accSetupId, short currencyId, short glId, string companyId)
        {
            if (accSetupId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.AccountSetup);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _accountSetupService.DeleteAccountSetupDtAsync(companyIdShort, parsedUserId.Value, accSetupId, currencyId, glId);
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

        [HttpGet]
        public async Task<JsonResult> AccountSetupCategoryList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
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
        public async Task<JsonResult> GetAccountSetupCategoryById(short accSetupCategoryId, string companyId)
        {
            if (accSetupCategoryId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
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
        public async Task<IActionResult> SaveAccountSetupCategory([FromBody] SaveAccountSetupCategoryViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
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
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
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
        public async Task<IActionResult> DeleteAccountSetupCategory(short accSetupCategoryId, string companyId)
        {
            if (accSetupCategoryId <= 0)
                return Json(new { success = false, message = "Invalid Account Setup Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.AccountSetupCategory);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _accountSetupService.DeleteAccountSetupCategoryAsync(companyIdShort, parsedUserId.Value, accSetupCategoryId);
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