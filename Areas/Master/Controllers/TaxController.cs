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
    public class TaxController : BaseController
    {
        private readonly ILogger<TaxController> _logger;
        private readonly ITaxService _taxService;

        public TaxController(ILogger<TaxController> logger,
            IBaseService baseService,
            ITaxService taxService)
            : base(logger, baseService)
        {
            _logger = logger;
            _taxService = taxService;
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
                (short)E_Modules.Master, (short)E_Master.Tax);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region Tax CRUD

        [HttpGet]
        public async Task<JsonResult> TaxList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taxService.GetTaxListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching GST list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTaxById(short taxId, string companyId)
        {
            if (taxId <= 0)
                return Json(new { success = false, message = "Invalid GST ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taxService.GetTaxByIdAsync(companyIdShort, parsedUserId.Value, taxId);
                return data == null
                    ? Json(new { success = false, message = "GST not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching GST by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveTax([FromBody] SaveTaxViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var taxToSave = new M_Tax
                {
                    TaxId = model.tax.TaxId,
                    CompanyId = companyIdShort,
                    TaxCategoryId = model.tax.TaxCategoryId,
                    TaxCode = model.tax.TaxCode ?? string.Empty,
                    TaxName = model.tax.TaxName ?? string.Empty,
                    Remarks = model.tax.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.tax.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _taxService.SaveTaxAsync(companyIdShort, parsedUserId.Value, taxToSave);
                return Json(new { success = true, message = "GST saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving GST");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTax(short taxId, string companyId)
        {
            if (taxId <= 0)
                return Json(new { success = false, message = "Invalid GST ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Tax);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _taxService.DeleteTaxAsync(companyIdShort, parsedUserId.Value, taxId);
                return Json(new { success = true, message = "GST deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting GST");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Tax CRUD

        #region TaxDt CRUD

        [HttpGet]
        public async Task<JsonResult> TaxDtList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taxService.GetTaxDtListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching GST details list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTaxDtById(short taxId, DateTime validFrom, string companyId)
        {
            if (taxId <= 0)
                return Json(new { success = false, message = "Invalid GST Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taxService.GetTaxDtByIdAsync(companyIdShort, parsedUserId.Value, taxId, validFrom);
                return data == null
                    ? Json(new { success = false, message = "GST Detail not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching GST detail by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveTaxDt([FromBody] SaveTaxDtViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var taxDtToSave = new M_TaxDt
                {
                    TaxId = model.taxDt.TaxId,
                    CompanyId = companyIdShort,
                    TaxPercentage = model.taxDt.TaxPercentage,
                    ValidFrom = model.taxDt.ValidFrom,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _taxService.SaveTaxDtAsync(companyIdShort, parsedUserId.Value, taxDtToSave);
                return Json(new { success = true, message = "GST Detail saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving GST detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTaxDt(short taxId, DateTime validFrom, string companyId)
        {
            if (taxId <= 0)
                return Json(new { success = false, message = "Invalid GST Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Tax);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _taxService.DeleteTaxDtAsync(companyIdShort, parsedUserId.Value, taxId, validFrom);
                return Json(new { success = true, message = "GST Detail deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting GST detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion TaxDt CRUD

        #region TaxCategory CRUD

        [HttpGet]
        public async Task<JsonResult> TaxCategoryList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taxService.GetTaxCategoryListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching GST category list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTaxCategoryById(short taxCategoryId, string companyId)
        {
            if (taxCategoryId <= 0)
                return Json(new { success = false, message = "Invalid GST Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taxService.GetTaxCategoryByIdAsync(companyIdShort, parsedUserId.Value, taxCategoryId);
                return data == null
                    ? Json(new { success = false, message = "GST Category not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching GST category by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveTaxCategory([FromBody] SaveTaxCategoryViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var categoryToSave = new M_TaxCategory
                {
                    TaxCategoryId = model.taxCategory.TaxCategoryId,
                    CompanyId = companyIdShort,
                    TaxCategoryCode = model.taxCategory.TaxCategoryCode ?? string.Empty,
                    TaxCategoryName = model.taxCategory.TaxCategoryName ?? string.Empty,
                    Remarks = model.taxCategory.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.taxCategory.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _taxService.SaveTaxCategoryAsync(companyIdShort, parsedUserId.Value, categoryToSave);
                return Json(new { success = true, message = "GST Category saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving GST category");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTaxCategory(short taxCategoryId, string companyId)
        {
            if (taxCategoryId <= 0)
                return Json(new { success = false, message = "Invalid GST Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.TaxCategory);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _taxService.DeleteTaxCategoryAsync(companyIdShort, parsedUserId.Value, taxCategoryId);
                return Json(new { success = true, message = "GST Category deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting GST category");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion TaxCategory CRUD
    }
}