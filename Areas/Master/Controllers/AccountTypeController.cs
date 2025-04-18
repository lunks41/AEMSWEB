﻿using AMESWEB.Areas.Master.Data.IServices;
using AMESWEB.Controllers;
using AMESWEB.Entities.Masters;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class AccountTypeController : BaseController
    {
        private readonly ILogger<AccountTypeController> _logger;
        private readonly IAccountTypeService _accountTypeService;

        public AccountTypeController(ILogger<AccountTypeController> logger,
            IBaseService baseService,
            IAccountTypeService accountTypeService)
            : base(logger, baseService)
        {
            _logger = logger;
            _accountTypeService = accountTypeService;
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
                (short)E_Modules.Master, (short)E_Master.AccountType);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region AccountType CRUD

        [HttpGet]
        public async Task<JsonResult> AccountTypeList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _accountTypeService.GetAccountTypeListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account type list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAccountTypeById(short accTypeId, string companyId)
        {
            if (accTypeId <= 0)
                return Json(new { success = false, message = "Invalid Account Type ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _accountTypeService.GetAccountTypeByIdAsync(companyIdShort, parsedUserId.Value, accTypeId);
                return data == null
                    ? Json(new { success = false, message = "Account Type not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account type by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAccountType([FromBody] SaveAccountTypeViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var accountTypeToSave = new M_AccountType
                {
                    AccTypeId = model.accountType.AccTypeId,
                    CompanyId = companyIdShort,
                    AccTypeCode = model.accountType.AccTypeCode ?? string.Empty,
                    AccTypeName = model.accountType.AccTypeName ?? string.Empty,
                    CodeStart = model.accountType.CodeStart,
                    CodeEnd = model.accountType.CodeEnd,
                    SeqNo = model.accountType.SeqNo,
                    AccGroupName = model.accountType.AccGroupName ?? string.Empty,
                    Remarks = model.accountType.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.accountType.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _accountTypeService.SaveAccountTypeAsync(companyIdShort, parsedUserId.Value, accountTypeToSave);
                return Json(new { success = true, message = "Account Type saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving account type");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccountType(short accTypeId, string companyId)
        {
            if (accTypeId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Account Type ID {AccTypeId}.", accTypeId);
                return Json(new { success = false, message = "Invalid Account Type ID." });
            }

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
            {
                return validationResult;
            }

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value, (short)E_Modules.Master, (short)E_Master.AccountType);
            if (permissions == null || !permissions.IsDelete)
            {
                _logger.LogWarning("Delete failed: User ID {UserId} does not have delete permissions.", parsedUserId.Value);
                return Json(new { success = false, message = "You do not have permission to delete this account group." });
            }

            try
            {
                await _accountTypeService.DeleteAccountTypeAsync(companyIdShort, parsedUserId.Value, accTypeId);
                return Json(new { success = true, message = "Account Type deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Account Type. Account Type ID: {AccTypeId}, Company ID: {CompanyId}.", accTypeId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion AccountType CRUD
    }
}