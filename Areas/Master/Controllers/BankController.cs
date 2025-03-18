﻿using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Areas.Master.Models;
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
    public class BankController : BaseController
    {
        private readonly ILogger<BankController> _logger;
        private readonly IBankService _bankService;

        public BankController(ILogger<BankController> logger,
            IBaseService baseService,
            IBankService bankService)
            : base(logger, baseService)
        {
            _logger = logger;
            _bankService = bankService;
        }

        #region Bank CRUD

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
                (short)E_Modules.Master, (short)E_Master.Bank);

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
                var data = await _bankService.GetBankListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bank list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(short bankId, string companyId)
        {
            if (bankId <= 0)
                return Json(new { success = false, message = "Invalid Bank ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _bankService.GetBankByIdAsync(companyIdShort, parsedUserId.Value, bankId);
                return data == null
                    ? Json(new { success = false, message = "Bank not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bank by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveBankViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var bankToSave = new M_Bank
                {
                    BankId = model.bank.BankId,
                    CompanyId = companyIdShort,
                    BankCode = model.bank.BankCode ?? string.Empty,
                    BankName = model.bank.BankName ?? string.Empty,
                    CurrencyId = model.bank.CurrencyId,
                    AccountNo = model.bank.AccountNo ?? string.Empty,
                    SwiftCode = model.bank.SwiftCode ?? string.Empty,
                    Remarks1 = model.bank.Remarks1?.Trim() ?? string.Empty,
                    Remarks2 = model.bank.Remarks2?.Trim() ?? string.Empty,
                    GLId = model.bank.GLId,
                    IsActive = model.bank.IsActive,
                    IsOwnBank = model.bank.IsOwnBank,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = model.bank.EditById ?? 0,
                    EditDate = DateTime.UtcNow
                };

                var result = await _bankService.SaveBankAsync(companyIdShort, parsedUserId.Value, bankToSave);
                return Json(new { success = true, message = "Bank saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving bank");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short bankId, string companyId)
        {
            if (bankId <= 0)
                return Json(new { success = false, message = "Invalid Bank ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Bank);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _bankService.DeleteBankAsync(companyIdShort, parsedUserId.Value, bankId);
                return Json(new { success = true, message = "Bank deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bank");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Bank CRUD
    }
}