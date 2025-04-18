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
    public class CurrencyController : BaseController
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ILogger<CurrencyController> logger,
            IBaseService baseService,
            ICurrencyService currencyService)
            : base(logger, baseService)
        {
            _logger = logger;
            _currencyService = currencyService;
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
                (short)E_Modules.Master, (short)E_Master.Currency);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region Currency CRUD

        [HttpGet]
        public async Task<JsonResult> CurrencyList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _currencyService.GetCurrencyListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching currency list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCurrencyById(short currencyId, string companyId)
        {
            if (currencyId <= 0)
                return Json(new { success = false, message = "Invalid Currency ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _currencyService.GetCurrencyByIdAsync(companyIdShort, parsedUserId.Value, currencyId);
                return data == null
                    ? Json(new { success = false, message = "Currency not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching currency by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCurrency([FromBody] SaveCurrencyViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var currencyToSave = new M_Currency
                {
                    CurrencyId = model.currency.CurrencyId,
                    CompanyId = companyIdShort,
                    CurrencyCode = model.currency.CurrencyCode ?? string.Empty,
                    CurrencyName = model.currency.CurrencyName ?? string.Empty,
                    IsMultiply = model.currency.IsMultiply,
                    Remarks = model.currency.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.currency.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _currencyService.SaveCurrencyAsync(companyIdShort, parsedUserId.Value, currencyToSave);
                return Json(new { success = true, message = "Currency saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving currency");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCurrency(short currencyId, string companyId)
        {
            if (currencyId <= 0)
                return Json(new { success = false, message = "Invalid Currency ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Currency);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _currencyService.DeleteCurrencyAsync(companyIdShort, parsedUserId.Value, currencyId);
                return Json(new { success = true, message = "Currency deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting currency");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Currency CRUD

        #region CurrencyDt CRUD

        [HttpGet]
        public async Task<JsonResult> CurrencyDtList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _currencyService.GetCurrencyDtListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching currency details list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCurrencyDtById(short currencyId, DateTime validFrom, string companyId)
        {
            if (currencyId <= 0)
                return Json(new { success = false, message = "Invalid Currency Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _currencyService.GetCurrencyDtByIdAsync(companyIdShort, parsedUserId.Value, currencyId, validFrom);
                return data == null
                    ? Json(new { success = false, message = "Currency Detail not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching currency detail by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCurrencyDt([FromBody] SaveCurrencyDtViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var detailToSave = new M_CurrencyDt
                {
                    CurrencyId = model.currencyDt.CurrencyId,
                    CompanyId = companyIdShort,
                    ExhRate = model.currencyDt.ExhRate,
                    ValidFrom = model.currencyDt.ValidFrom, // Assuming ValidFrom is string in ViewModel
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _currencyService.SaveCurrencyDtAsync(companyIdShort, parsedUserId.Value, detailToSave);
                return Json(new { success = true, message = "Currency Detail saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving currency detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCurrencyDt(short currencyId, DateTime validFrom, string companyId)
        {
            if (currencyId <= 0)
                return Json(new { success = false, message = "Invalid Currency Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Currency);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var detail = await _currencyService.GetCurrencyDtByIdAsync(companyIdShort, parsedUserId.Value, currencyId, validFrom);
                if (detail == null)
                    return Json(new { success = false, message = "Currency Detail not found" });

                await _currencyService.DeleteCurrencyDtAsync(companyIdShort, parsedUserId.Value, detail);
                return Json(new { success = true, message = "Currency Detail deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting currency detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion CurrencyDt CRUD

        #region CurrencyLocalDt CRUD

        [HttpGet]
        public async Task<JsonResult> CurrencyLocalDtList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _currencyService.GetCurrencyLocalDtListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching local currency details list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCurrencyLocalDtById(short currencyId, DateTime validFrom, string companyId)
        {
            if (currencyId <= 0)
                return Json(new { success = false, message = "Invalid Local Currency Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _currencyService.GetCurrencyLocalDtByIdAsync(companyIdShort, parsedUserId.Value, currencyId, validFrom);
                return data == null
                    ? Json(new { success = false, message = "Local Currency Detail not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching local currency detail by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCurrencyLocalDt([FromBody] SaveCurrencyLocalDtViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var localDetailToSave = new M_CurrencyLocalDt
                {
                    CurrencyId = model.currencyLocalDt.CurrencyId,
                    CompanyId = companyIdShort,
                    ExhRate = model.currencyLocalDt.ExhRate,
                    ValidFrom = model.currencyLocalDt.ValidFrom, // String to DateTime conversion
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _currencyService.SaveCurrencyLocalDtAsync(companyIdShort, parsedUserId.Value, localDetailToSave);
                return Json(new { success = true, message = "Local Currency Detail saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving local currency detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCurrencyLocalDt(short currencyId, DateTime validFrom, string companyId)
        {
            if (currencyId <= 0)
                return Json(new { success = false, message = "Invalid Local Currency Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Currency);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var localDetail = await _currencyService.GetCurrencyLocalDtByIdAsync(companyIdShort, parsedUserId.Value, currencyId, validFrom);
                if (localDetail == null)
                    return Json(new { success = false, message = "Local Currency Detail not found" });

                await _currencyService.DeleteCurrencyLocalDtAsync(companyIdShort, parsedUserId.Value, localDetail);
                return Json(new { success = true, message = "Local Currency Detail deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting local currency detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion CurrencyLocalDt CRUD
    }
}