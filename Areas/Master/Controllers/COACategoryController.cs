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
    public class COACategoryController : BaseController
    {
        private readonly ILogger<COACategoryController> _logger;
        private readonly ICOACategoryService _coaCategoryService;

        public COACategoryController(ILogger<COACategoryController> logger, IBaseService baseService, ICOACategoryService coaCategoryService) : base(logger, baseService)
        {
            _logger = logger;
            _coaCategoryService = coaCategoryService;
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
                (short)E_Modules.Master, (short)E_Master.COACategory1);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region COACategory1 CRUD

        [HttpGet]
        public async Task<JsonResult> COACategory1List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _coaCategoryService.GetCOACategory1ListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching barge list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCOACategory1ById(short coaCategoryId, string companyId)
        {
            if (coaCategoryId <= 0)
                return Json(new { success = false, message = "Invalid COACategory ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _coaCategoryService.GetCOACategory1ByIdAsync(companyIdShort, parsedUserId.Value, coaCategoryId);
                return data == null
                    ? Json(new { success = false, message = "COACategory not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching barge by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCOACategory1([FromBody] SaveCOACategoryViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var coaCategoryToSave = new M_COACategory1
                {
                    COACategoryId = model.coaCategory.COACategoryId,
                    CompanyId = companyIdShort,
                    COACategoryCode = model.coaCategory.COACategoryCode ?? string.Empty,
                    COACategoryName = model.coaCategory.COACategoryName ?? string.Empty,
                    SeqNo = model.coaCategory.SeqNo,
                    Remarks = model.coaCategory.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.coaCategory.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _coaCategoryService.SaveCOACategory1Async(companyIdShort, parsedUserId.Value, coaCategoryToSave);
                return Json(new { success = true, message = "COACategory saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving barge");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCOACategory1(short coaCategoryId, string companyId)
        {
            if (coaCategoryId <= 0)
                return Json(new { success = false, message = "Invalid COACategory ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.COACategory1);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _coaCategoryService.DeleteCOACategory1Async(companyIdShort, parsedUserId.Value, coaCategoryId);
                return Json(new { success = true, message = "COACategory deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting barge");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion COACategory1 CRUD

        #region COACategory2 CRUD

        [HttpGet]
        public async Task<JsonResult> COACategory2List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _coaCategoryService.GetCOACategory2ListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching barge list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCOACategory2ById(short coaCategoryId, string companyId)
        {
            if (coaCategoryId <= 0)
                return Json(new { success = false, message = "Invalid COACategory ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _coaCategoryService.GetCOACategory2ByIdAsync(companyIdShort, parsedUserId.Value, coaCategoryId);
                return data == null
                    ? Json(new { success = false, message = "COACategory not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching barge by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCOACategory2([FromBody] SaveCOACategoryViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var coaCategoryToSave = new M_COACategory2
                {
                    COACategoryId = model.coaCategory.COACategoryId,
                    CompanyId = companyIdShort,
                    COACategoryCode = model.coaCategory.COACategoryCode ?? string.Empty,
                    COACategoryName = model.coaCategory.COACategoryName ?? string.Empty,
                    SeqNo = model.coaCategory.SeqNo,
                    Remarks = model.coaCategory.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.coaCategory.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _coaCategoryService.SaveCOACategory2Async(companyIdShort, parsedUserId.Value, coaCategoryToSave);
                return Json(new { success = true, message = "COACategory saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving barge");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCOACategory2(short coaCategoryId, string companyId)
        {
            if (coaCategoryId <= 0)
                return Json(new { success = false, message = "Invalid COACategory ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.COACategory2);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _coaCategoryService.DeleteCOACategory2Async(companyIdShort, parsedUserId.Value, coaCategoryId);
                return Json(new { success = true, message = "COACategory deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting barge");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion COACategory2 CRUD

        #region COACategory3 CRUD

        [HttpGet]
        public async Task<JsonResult> COACategory3List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _coaCategoryService.GetCOACategory3ListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching barge list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCOACategory3ById(short coaCategoryId, string companyId)
        {
            if (coaCategoryId <= 0)
                return Json(new { success = false, message = "Invalid COACategory ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _coaCategoryService.GetCOACategory3ByIdAsync(companyIdShort, parsedUserId.Value, coaCategoryId);
                return data == null
                    ? Json(new { success = false, message = "COACategory not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching barge by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCOACategory3([FromBody] SaveCOACategoryViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var coaCategoryToSave = new M_COACategory3
                {
                    COACategoryId = model.coaCategory.COACategoryId,
                    CompanyId = companyIdShort,
                    COACategoryCode = model.coaCategory.COACategoryCode ?? string.Empty,
                    COACategoryName = model.coaCategory.COACategoryName ?? string.Empty,
                    SeqNo = model.coaCategory.SeqNo,
                    Remarks = model.coaCategory.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.coaCategory.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _coaCategoryService.SaveCOACategory3Async(companyIdShort, parsedUserId.Value, coaCategoryToSave);
                return Json(new { success = true, message = "COACategory saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving barge");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCOACategory3(short coaCategoryId, string companyId)
        {
            if (coaCategoryId <= 0)
                return Json(new { success = false, message = "Invalid COACategory ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.COACategory3);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _coaCategoryService.DeleteCOACategory3Async(companyIdShort, parsedUserId.Value, coaCategoryId);
                return Json(new { success = true, message = "COACategory deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting barge");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion COACategory3 CRUD
    }
}