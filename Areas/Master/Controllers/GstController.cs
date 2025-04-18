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
    public class GstController : BaseController
    {
        private readonly ILogger<GstController> _logger;
        private readonly IGstService _gstService;

        public GstController(ILogger<GstController> logger,
            IBaseService baseService,
            IGstService gstService)
            : base(logger, baseService)
        {
            _logger = logger;
            _gstService = gstService;
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
                (short)E_Modules.Master, (short)E_Master.Gst);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region Gst CRUD

        [HttpGet]
        public async Task<JsonResult> GstList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _gstService.GetGstListAsync(companyIdShort, parsedUserId.Value,
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
        public async Task<JsonResult> GetGstById(short gstId, string companyId)
        {
            if (gstId <= 0)
                return Json(new { success = false, message = "Invalid GST ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _gstService.GetGstByIdAsync(companyIdShort, parsedUserId.Value, gstId);
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
        public async Task<IActionResult> SaveGst([FromBody] SaveGstViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var gstToSave = new M_Gst
                {
                    GstId = model.gst.GstId,
                    CompanyId = companyIdShort,
                    GstCategoryId = model.gst.GstCategoryId,
                    GstCode = model.gst.GstCode ?? string.Empty,
                    GstName = model.gst.GstName ?? string.Empty,
                    Remarks = model.gst.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.gst.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _gstService.SaveGstAsync(companyIdShort, parsedUserId.Value, gstToSave);
                return Json(new { success = true, message = "GST saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving GST");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGst(short gstId, string companyId)
        {
            if (gstId <= 0)
                return Json(new { success = false, message = "Invalid GST ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Gst);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _gstService.DeleteGstAsync(companyIdShort, parsedUserId.Value, gstId);
                return Json(new { success = true, message = "GST deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting GST");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Gst CRUD

        #region GstDt CRUD

        [HttpGet]
        public async Task<JsonResult> GstDtList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _gstService.GetGstDtListAsync(companyIdShort, parsedUserId.Value,
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
        public async Task<JsonResult> GetGstDtById(short gstId, DateTime validFrom, string companyId)
        {
            if (gstId <= 0)
                return Json(new { success = false, message = "Invalid GST Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _gstService.GetGstDtByIdAsync(companyIdShort, parsedUserId.Value, gstId, validFrom);
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
        public async Task<IActionResult> SaveGstDt([FromBody] SaveGstDtViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var gstDtToSave = new M_GstDt
                {
                    GstId = model.gstDt.GstId,
                    CompanyId = companyIdShort,
                    GstPercentage = model.gstDt.GstPercentage,
                    ValidFrom = model.gstDt.ValidFrom,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _gstService.SaveGstDtAsync(companyIdShort, parsedUserId.Value, gstDtToSave);
                return Json(new { success = true, message = "GST Detail saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving GST detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGstDt(short gstId, DateTime validFrom, string companyId)
        {
            if (gstId <= 0)
                return Json(new { success = false, message = "Invalid GST Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Gst);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _gstService.DeleteGstDtAsync(companyIdShort, parsedUserId.Value, gstId, validFrom);
                return Json(new { success = true, message = "GST Detail deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting GST detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion GstDt CRUD

        #region GstCategory CRUD

        [HttpGet]
        public async Task<JsonResult> GstCategoryList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _gstService.GetGstCategoryListAsync(companyIdShort, parsedUserId.Value,
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
        public async Task<JsonResult> GetGstCategoryById(short gstCategoryId, string companyId)
        {
            if (gstCategoryId <= 0)
                return Json(new { success = false, message = "Invalid GST Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _gstService.GetGstCategoryByIdAsync(companyIdShort, parsedUserId.Value, gstCategoryId);
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
        public async Task<IActionResult> SaveGstCategory([FromBody] SaveGstCategoryViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var categoryToSave = new M_GstCategory
                {
                    GstCategoryId = model.gstCategory.GstCategoryId,
                    CompanyId = companyIdShort,
                    GstCategoryCode = model.gstCategory.GstCategoryCode ?? string.Empty,
                    GstCategoryName = model.gstCategory.GstCategoryName ?? string.Empty,
                    Remarks = model.gstCategory.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.gstCategory.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _gstService.SaveGstCategoryAsync(companyIdShort, parsedUserId.Value, categoryToSave);
                return Json(new { success = true, message = "GST Category saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving GST category");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGstCategory(short gstCategoryId, string companyId)
        {
            if (gstCategoryId <= 0)
                return Json(new { success = false, message = "Invalid GST Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.GstCategory);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _gstService.DeleteGstCategoryAsync(companyIdShort, parsedUserId.Value, gstCategoryId);
                return Json(new { success = true, message = "GST Category deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting GST category");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion GstCategory CRUD
    }
}