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
    public class CountryController : BaseController
    {
        private readonly ILogger<CountryController> _logger;
        private readonly ICountryService _countryService;

        public CountryController(ILogger<CountryController> logger,
            IBaseService baseService,
            ICountryService countryService)
            : base(logger, baseService)
        {
            _logger = logger;
            _countryService = countryService;
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
                (short)E_Modules.Master, (short)E_Master.Country);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region Country CRUD

        [HttpGet]
        public async Task<JsonResult> CountryList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _countryService.GetCountryListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching country list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCountryById(short countryId, string companyId)
        {
            if (countryId <= 0)
                return Json(new { success = false, message = "Invalid Country ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _countryService.GetCountryByIdAsync(companyIdShort, parsedUserId.Value, countryId);
                return data == null
                    ? Json(new { success = false, message = "Country not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching country by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCountry([FromBody] SaveCountryViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var countryToSave = new M_Country
                {
                    CountryId = model.country.CountryId,
                    CompanyId = companyIdShort,
                    CountryCode = model.country.CountryCode ?? string.Empty,
                    CountryName = model.country.CountryName ?? string.Empty,
                    Remarks = model.country.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.country.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = model.country.EditById ?? 0,
                    EditDate = DateTime.UtcNow
                };

                var result = await _countryService.SaveCountryAsync(companyIdShort, parsedUserId.Value, countryToSave);
                return Json(new { success = true, message = "Country saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving country");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCountry(short countryId, string companyId)
        {
            if (countryId <= 0)
                return Json(new { success = false, message = "Invalid Country ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Country);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _countryService.DeleteCountryAsync(companyIdShort, parsedUserId.Value, countryId);
                return Json(new { success = true, message = "Country deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting country");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Country CRUD
    }
}