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
    public class AccountGroupController : BaseController
    {
        private readonly ILogger<AccountGroupController> _logger;
        private readonly IAccountGroupService _AccountGroupService;

        // Constructor with dependency injection
        public AccountGroupController(ILogger<AccountGroupController> logger, IBaseService baseService, IAccountGroupService AccountGroupService) : base(logger, baseService)
        {
            _logger = logger;
            _AccountGroupService = AccountGroupService;
        }

        // Centralized validation for companyId and userId

        // Index action to render the view
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

            var permissions = await HasPermission((short)companyId, parsedUserId.Value, (short)E_Modules.Master, (short)E_Master.AccountGroup);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.IsExport = permissions?.IsExport ?? false;
            ViewBag.IsPrint = permissions?.IsPrint ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        // GET: List account groups with pagination and search
        [HttpGet]
        public async Task<JsonResult> List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return Json(new { success = false, message = "Invalid page number or page size." });
            }

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null)
            {
                return Json(validationResult);
            }

            try
            {
                var data = await _AccountGroupService.GetAccountGroupListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });

                //var total = data.totalRecords;
                //var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                //return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account groups.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // GET: Retrieve a specific account group by ID
        [HttpGet]
        public async Task<JsonResult> GetById(short accGroupId, string companyId)
        {
            if (accGroupId <= 0)
            {
                return Json(new { success = false, message = "Invalid Account Group ID." });
            }

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null)
            {
                return Json(validationResult);
            }

            try
            {
                var data = await _AccountGroupService.GetAccountGroupByIdAsync(companyIdShort, parsedUserId.Value, accGroupId);
                if (data == null)
                {
                    return Json(new { success = false, message = "Account Group not found." });
                }
                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account group by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: Save or update an account group
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveAccountGroupViewModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                _logger.LogWarning("Save failed: Invalid or null model.");
                return Json(new { success = false, message = "Invalid request data." });
            }

            var validationResult = ValidateCompanyAndUserId(model.CompanyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                var accountGroupToSave = new M_AccountGroup
                {
                    AccGroupId = model.accountGroup.AccGroupId,
                    CompanyId = companyIdShort,
                    AccGroupCode = model.accountGroup.AccGroupCode ?? string.Empty,
                    AccGroupName = model.accountGroup.AccGroupName ?? string.Empty,
                    SeqNo = model.accountGroup.SeqNo,
                    Remarks = model.accountGroup.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.accountGroup.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = model.accountGroup.EditById ?? 0,
                    EditDate = DateTime.UtcNow
                };

                var data = await _AccountGroupService.SaveAccountGroupAsync(companyIdShort, parsedUserId.Value, accountGroupToSave);
                return Json(new { success = true, message = "Account Group saved successfully.", data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        // DELETE: Remove an account group
        [HttpDelete]
        public async Task<IActionResult> Delete(short accGroupId, string companyId)
        {
            if (accGroupId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Account Group ID {AccGroupId}.", accGroupId);
                return Json(new { success = false, message = "Invalid Account Group ID." });
            }

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null)
            {
                return validationResult;
            }

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value, (short)E_Modules.Master, (short)E_Master.AccountGroup);
            if (permissions == null || !permissions.IsDelete)
            {
                _logger.LogWarning("Delete failed: User ID {UserId} does not have delete permissions.", parsedUserId.Value);
                return Json(new { success = false, message = "You do not have permission to delete this account group." });
            }

            try
            {
                var accountGroup = await _AccountGroupService.GetAccountGroupByIdAsync(companyIdShort, parsedUserId!.Value, accGroupId);
                if (accountGroup == null)
                {
                    _logger.LogWarning("Delete failed: Account Group with ID {AccGroupId} not found for Company ID {CompanyId}.", accGroupId, companyIdShort);
                    return Json(new { success = false, message = "Account Group not found." });
                }

                await _AccountGroupService.DeleteAccountGroupAsync(companyIdShort, parsedUserId.Value, accountGroup);
                return Json(new { success = true, message = "Account Group deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Account Group. Account Group ID: {AccGroupId}, Company ID: {CompanyId}.", accGroupId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }
    }
}