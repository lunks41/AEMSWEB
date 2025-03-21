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

        public TaxController(ILogger<TaxController> logger, IBaseService baseService, ITaxService taxService)
            : base(logger, baseService)
        {
            _logger = logger;
            _taxService = taxService;
        }

        #region Tax CRUD

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

        [HttpGet]
        public async Task<JsonResult> List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taxService.GetTaxListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tax list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(short taxId, string companyId)
        {
            if (taxId <= 0)
                return Json(new { success = false, message = "Invalid Tax ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taxService.GetTaxByIdAsync(companyIdShort, parsedUserId.Value, taxId);
                return data == null
                    ? Json(new { success = false, message = "Tax not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tax by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveTaxViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
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
                    CreateDate = DateTime.UtcNow,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.UtcNow
                };

                var result = await _taxService.SaveTaxAsync(companyIdShort, parsedUserId.Value, taxToSave);
                return Json(new { success = true, message = "Tax saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving tax");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short taxId, string companyId)
        {
            if (taxId <= 0)
                return Json(new { success = false, message = "Invalid Tax ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Tax);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _taxService.DeleteTaxAsync(companyIdShort, parsedUserId.Value, taxId);
                return Json(new { success = true, message = "Tax deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tax");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Tax CRUD

        #region TaxDt CRUD

        [HttpGet]
        public async Task<JsonResult> ListDetails(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taxService.GetTaxDtListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tax details list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetDetailById(short taxDtId, DateTime validFrom, string companyId)
        {
            if (taxDtId <= 0)
                return Json(new { success = false, message = "Invalid Tax Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taxService.GetTaxDtByIdAsync(companyIdShort, parsedUserId.Value, taxDtId, validFrom);
                return data == null
                    ? Json(new { success = false, message = "Tax Detail not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tax detail by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveDetail([FromBody] SaveTaxDtViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
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
                    CreateDate = DateTime.UtcNow,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.UtcNow
                };

                var result = await _taxService.SaveTaxDtAsync(companyIdShort, parsedUserId.Value, taxDtToSave);
                return Json(new { success = true, message = "Tax Detail saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving tax detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDetail(short taxDtId, DateTime validFrom, string companyId)
        {
            if (taxDtId <= 0)
                return Json(new { success = false, message = "Invalid Tax Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Tax);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _taxService.DeleteTaxDtAsync(companyIdShort, parsedUserId.Value, taxDtId, validFrom);
                return Json(new { success = true, message = "Tax Detail deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tax detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion TaxDt CRUD

        #region TaxCategory CRUD

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
                (short)E_Modules.Master, (short)E_Master.TaxCategory);

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
                var data = await _taxService.GetTaxCategoryListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tax category list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCategoryById(short taxCategoryId, string companyId)
        {
            if (taxCategoryId <= 0)
                return Json(new { success = false, message = "Invalid Tax Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taxService.GetTaxCategoryByIdAsync(companyIdShort, parsedUserId.Value, taxCategoryId);
                return data == null
                    ? Json(new { success = false, message = "Tax Category not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tax category by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCategory([FromBody] SaveTaxCategoryViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
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
                    CreateDate = DateTime.UtcNow,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.UtcNow
                };

                var result = await _taxService.SaveTaxCategoryAsync(companyIdShort, parsedUserId.Value, categoryToSave);
                return Json(new { success = true, message = "Tax Category saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving tax category");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(short taxCategoryId, string companyId)
        {
            if (taxCategoryId <= 0)
                return Json(new { success = false, message = "Invalid Tax Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.TaxCategory);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _taxService.DeleteTaxCategoryAsync(companyIdShort, parsedUserId.Value, taxCategoryId);
                return Json(new { success = true, message = "Tax Category deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tax category");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion TaxCategory CRUD
    }
}