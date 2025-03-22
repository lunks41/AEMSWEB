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
    public class UomController : BaseController
    {
        private readonly ILogger<UomController> _logger;
        private readonly IUomService _uomService;

        public UomController(ILogger<UomController> logger, IBaseService baseService, IUomService uomService)
            : base(logger, baseService)
        {
            _logger = logger;
            _uomService = uomService;
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
                (short)E_Modules.Master, (short)E_Master.Uom);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region Uom CRUD

        [HttpGet]
        public async Task<JsonResult> UomList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _uomService.GetUomListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching UOM list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUomById(short uomId, string companyId)
        {
            if (uomId <= 0)
                return Json(new { success = false, message = "Invalid UOM ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _uomService.GetUomByIdAsync(companyIdShort, parsedUserId.Value, uomId);
                return data == null
                    ? Json(new { success = false, message = "UOM not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching UOM by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUom([FromBody] SaveUomViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.CompanyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var uomToSave = new M_Uom
                {
                    UomId = model.Uom.UomId,
                    CompanyId = companyIdShort,
                    UomCode = model.Uom.UomCode ?? string.Empty,
                    UomName = model.Uom.UomName ?? string.Empty,
                    Remarks = model.Uom.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.Uom.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _uomService.SaveUomAsync(companyIdShort, parsedUserId.Value, uomToSave);
                return Json(new { success = true, message = "UOM saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving UOM");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUom(short uomId, string companyId)
        {
            if (uomId <= 0)
                return Json(new { success = false, message = "Invalid UOM ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Uom);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _uomService.DeleteUomAsync(companyIdShort, parsedUserId.Value, uomId);
                return Json(new { success = true, message = "UOM deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting UOM");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Uom CRUD

        #region UomDt CRUD

        [HttpGet]
        public async Task<JsonResult> UomDtList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _uomService.GetUomDtListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching UOM details list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUomDtById(short uomId, short packUomId, string companyId)
        {
            if (uomId <= 0)
                return Json(new { success = false, message = "Invalid UOM Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _uomService.GetUomDtByIdAsync(companyIdShort, parsedUserId.Value, uomId, packUomId);
                return data == null
                    ? Json(new { success = false, message = "UOM Detail not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching UOM Detail by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUomDt([FromBody] SaveUomDtViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var uomDtToSave = new M_UomDt
                {
                    CompanyId = companyIdShort,
                    UomId = model.uomDt.UomId,
                    PackUomId = model.uomDt.PackUomId,
                    UomFactor = model.uomDt.UomFactor,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _uomService.SaveUomDtAsync(companyIdShort, parsedUserId.Value, uomDtToSave);
                return Json(new { success = true, message = "UOM Detail saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving UOM Detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUomDt(short uomId, short packUomId, string companyId)
        {
            if (uomId <= 0)
                return Json(new { success = false, message = "Invalid UOM Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Uom);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _uomService.DeleteUomDtAsync(companyIdShort, parsedUserId.Value, uomId, packUomId);
                return Json(new { success = true, message = "UOM Detail deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting UOM Detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion UomDt CRUD
    }
}