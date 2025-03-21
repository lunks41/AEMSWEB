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
    public class ChargesController : BaseController
    {
        private readonly ILogger<ChargesController> _logger;
        private readonly IChargesService _ChargesService;

        public ChargesController(ILogger<ChargesController> logger,
            IBaseService baseService,
            IChargesService ChargesService)
            : base(logger, baseService)
        {
            _logger = logger;
            _ChargesService = ChargesService;
        }

        #region Charges CRUD

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
                (short)E_Modules.Master, (short)E_Master.Charges);

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
                var data = await _ChargesService.GetChargesListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Charges list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(short ChargeId, string companyId)
        {
            if (ChargeId <= 0)
                return Json(new { success = false, message = "Invalid Charges ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _ChargesService.GetChargesByIdAsync(companyIdShort, parsedUserId.Value, ChargeId);
                return data == null
                    ? Json(new { success = false, message = "Charges not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Charges by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveChargesViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var ChargesToSave = new M_Charges
                {
                    ChargeId = model.charges.ChargeId,
                    ChargeCode = model.charges.ChargeCode ?? string.Empty,
                    ChargeName = model.charges.ChargeName ?? string.Empty,
                    Remarks = model.charges.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.charges.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.UtcNow
                };

                var result = await _ChargesService.SaveChargesAsync(companyIdShort, parsedUserId.Value, ChargesToSave);
                return Json(new { success = true, message = "Charges saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Charges");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short ChargeId, string companyId)
        {
            if (ChargeId <= 0)
                return Json(new { success = false, message = "Invalid Charges ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Charges);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var Charges = await _ChargesService.GetChargesByIdAsync(companyIdShort, parsedUserId.Value, ChargeId);
                if (Charges == null)
                    return Json(new { success = false, message = "Charges not found" });

                await _ChargesService.DeleteChargesAsync(companyIdShort, parsedUserId.Value, Charges);
                return Json(new { success = true, message = "Charges deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Charges");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Charges CRUD
    }
}