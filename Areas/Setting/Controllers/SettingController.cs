using AEMSWEB.Areas.Setting.Data;
using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Controllers;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AEMSWEB.Areas.Setting.Controllers
{
    [Area("setting")]
    [Authorize]
    public class SettingController : BaseController
    {
        private readonly ILogger<SettingController> _logger;
        private readonly ISettingService _settingService;

        public SettingController(ILogger<SettingController> logger, IBaseService baseService, ISettingService settingService) : base(logger, baseService)
        {
            _logger = logger;
            _settingService = settingService;
        }

        public async Task<IActionResult> Index()
        {
            var companyId = HttpContext.Session.GetString("CurrentCompany");
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "Setting not logged in or invalid user ID." });
            }
            var permissions = await HasPermission(companyIdShort, parsedUserId, (short)E_Modules.Setting, (short)E_Setting.DecSetting);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.IsExport = permissions?.IsExport ?? false;
            ViewBag.IsPrint = permissions?.IsPrint ?? false;

            return View();
        }

        #region Finance Setting

        [HttpGet]
        public async Task<JsonResult> GetDecSetting(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "Setting not logged in or invalid user ID." });
            }

            try
            {
                var data = await _settingService.GetDecSettingAsync(companyIdShort, parsedUserId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Decimal Setting not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account group by ID.");
                return Json(new { success = false, message = "An error occurred", data = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveDecSetting([FromBody] SaveDecimalSettingViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var decimalSetting = model.DecimalSetting;
            var companyId = model.CompanyId;

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "Setting not logged in or invalid user ID." });
            }

            var decimalSettingToSave = new S_DecSettings
            {
                CompanyId = companyIdShort,
                AmtDec = decimalSetting.AmtDec,
                LocAmtDec = decimalSetting.LocAmtDec,
                CtyAmtDec = decimalSetting.CtyAmtDec,
                PriceDec = decimalSetting.PriceDec,
                QtyDec = decimalSetting.QtyDec,
                ExhRateDec = decimalSetting.ExhRateDec,
                DateFormat = decimalSetting.DateFormat,
                LongDateFormat = decimalSetting.LongDateFormat,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = decimalSetting.EditById ?? parsedUserId,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _settingService.SaveDecSettingAsync(companyIdShort, parsedUserId, decimalSettingToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account group." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }

        #endregion Finance Setting

        #region Finance Setting

        [HttpGet]
        public async Task<JsonResult> GetFinSetting(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "Setting not logged in or invalid user ID." });
            }

            try
            {
                var data = await _settingService.GetFinSettingAsync(companyIdShort, parsedUserId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Finance Setting not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account group by ID.");
                return Json(new { success = false, message = "An error occurred", data = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveFinSetting([FromBody] SaveFinanceSettingViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var financeSetting = model.FinanceSetting;
            var companyId = model.CompanyId;

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "Setting not logged in or invalid user ID." });
            }

            var financeSettingToSave = new S_FinSettings
            {
                CompanyId = companyIdShort,
                Base_CurrencyId = financeSetting.Base_CurrencyId,
                Local_CurrencyId = financeSetting.Local_CurrencyId,
                ExhGainLoss_GlId = financeSetting.ExhGainLoss_GlId,
                BankCharge_GlId = financeSetting.BankCharge_GlId,
                ProfitLoss_GlId = financeSetting.ProfitLoss_GlId,
                RetEarning_GlId = financeSetting.ProfitLoss_GlId,
                SaleGst_GlId = financeSetting.SaleGst_GlId,
                PurGst_GlId = financeSetting.PurGst_GlId,
                SaleDef_GlId = financeSetting.SaleDef_GlId,
                PurDef_GlId = financeSetting.PurDef_GlId,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = financeSetting.EditById ?? parsedUserId,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _settingService.SaveFinSettingAsync(companyIdShort, parsedUserId, financeSettingToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account group." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }

        #endregion Finance Setting
    }
}