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
    public class PortRegionController : BaseController
    {
        private readonly ILogger<PortRegionController> _logger;
        private readonly IPortRegionService _portRegionService;

        public PortRegionController(ILogger<PortRegionController> logger,
            IBaseService baseService,
            IPortRegionService portRegionService)
            : base(logger, baseService)
        {
            _logger = logger;
            _portRegionService = portRegionService;
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
                (short)E_Modules.Master, (short)E_Master.PortRegion);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region PortRegion CRUD

        [HttpGet]
        public async Task<JsonResult> PortRegionList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _portRegionService.GetPortRegionListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching port region list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetPortRegionById(short portRegionId, string companyId)
        {
            if (portRegionId <= 0)
                return Json(new { success = false, message = "Invalid Port Region ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _portRegionService.GetPortRegionByIdAsync(companyIdShort, parsedUserId.Value, portRegionId);
                return data == null
                    ? Json(new { success = false, message = "Port Region not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching port region by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePortRegion([FromBody] SavePortRegionViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var portRegionToSave = new M_PortRegion
                {
                    PortRegionId = model.portRegion.PortRegionId,
                    CompanyId = companyIdShort,
                    PortRegionCode = model.portRegion.PortRegionCode ?? string.Empty,
                    PortRegionName = model.portRegion.PortRegionName ?? string.Empty,
                    Remarks = model.portRegion.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.portRegion.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _portRegionService.SavePortRegionAsync(companyIdShort, parsedUserId.Value, portRegionToSave);
                return Json(new { success = true, message = "Port Region saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving port region");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePortRegion(short portRegionId, string companyId)
        {
            if (portRegionId <= 0)
                return Json(new { success = false, message = "Invalid Port Region ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.PortRegion);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _portRegionService.DeletePortRegionAsync(companyIdShort, parsedUserId.Value, portRegionId);
                return Json(new { success = true, message = "Port Region deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting port region");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion PortRegion CRUD
    }
}