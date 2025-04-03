using AMESWEB.Areas.Master.Data.IServices;
using AMESWEB.Areas.Master.Models;
using AMESWEB.Controllers;
using AMESWEB.Entities.Masters;
using AMESWEB.Enums;
using AMESWEB.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class PortController : BaseController
    {
        private readonly ILogger<PortController> _logger;
        private readonly IPortService _portService;

        public PortController(ILogger<PortController> logger,
            IBaseService baseService,
            IPortService portService)
            : base(logger, baseService)
        {
            _logger = logger;
            _portService = portService;
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
                (short)E_Modules.Master, (short)E_Master.Port);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region Port CRUD

        [HttpGet]
        public async Task<JsonResult> PortList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _portService.GetPortListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching port list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetPortById(short portId, string companyId)
        {
            if (portId <= 0)
                return Json(new { success = false, message = "Invalid Port ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _portService.GetPortByIdAsync(companyIdShort, parsedUserId.Value, portId);
                return data == null
                    ? Json(new { success = false, message = "Port not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching port by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePort([FromBody] SavePortViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var portToSave = new M_Port
                {
                    PortId = model.port.PortId,
                    CompanyId = companyIdShort,
                    PortRegionId = model.port.PortRegionId,
                    PortCode = model.port.PortCode ?? string.Empty,
                    PortName = model.port.PortName ?? string.Empty,
                    Remarks = model.port.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.port.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _portService.SavePortAsync(companyIdShort, parsedUserId.Value, portToSave);
                return Json(new { success = true, message = "Port saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving port");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePort(short portId, string companyId)
        {
            if (portId <= 0)
                return Json(new { success = false, message = "Invalid Port ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Port);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _portService.DeletePortAsync(companyIdShort, parsedUserId.Value, portId);
                return Json(new { success = true, message = "Port deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting port");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Port CRUD
    }
}