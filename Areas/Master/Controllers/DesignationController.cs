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
    public class DesignationController : BaseController
    {
        private readonly ILogger<DesignationController> _logger;
        private readonly IDesignationService _designationService;

        public DesignationController(ILogger<DesignationController> logger,
            IBaseService baseService,
            IDesignationService designationService)
            : base(logger, baseService)
        {
            _logger = logger;
            _designationService = designationService;
        }

        #region Designation CRUD

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
                (short)E_Modules.Master, (short)E_Master.Designation);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> DesignationList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _designationService.GetDesignationListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching designation list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetDesignationById(short designationId, string companyId)
        {
            if (designationId <= 0)
                return Json(new { success = false, message = "Invalid Designation ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _designationService.GetDesignationByIdAsync(companyIdShort, parsedUserId.Value, designationId);
                return data == null
                    ? Json(new { success = false, message = "Designation not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching designation by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveDesignation([FromBody] SaveDesignationViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var designationToSave = new M_Designation
                {
                    DesignationId = model.designation.DesignationId,
                    CompanyId = companyIdShort,
                    DesignationCode = model.designation.DesignationCode ?? string.Empty,
                    DesignationName = model.designation.DesignationName ?? string.Empty,
                    Remarks = model.designation.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.designation.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _designationService.SaveDesignationAsync(companyIdShort, parsedUserId.Value, designationToSave);
                return Json(new { success = true, message = "Designation saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving designation");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDesignation(short designationId, string companyId)
        {
            if (designationId <= 0)
                return Json(new { success = false, message = "Invalid Designation ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Designation);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _designationService.DeleteDesignationAsync(companyIdShort, parsedUserId.Value, designationId);
                return Json(new { success = true, message = "Designation deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting designation");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Designation CRUD
    }
}