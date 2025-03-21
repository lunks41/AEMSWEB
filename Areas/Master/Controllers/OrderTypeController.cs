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
    public class OrderTypeController : BaseController
    {
        private readonly ILogger<OrderTypeController> _logger;
        private readonly IOrderTypeService _orderTypeService;

        public OrderTypeController(ILogger<OrderTypeController> logger,
            IBaseService baseService,
            IOrderTypeService orderTypeService)
            : base(logger, baseService)
        {
            _logger = logger;
            _orderTypeService = orderTypeService;
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
                (short)E_Modules.Master, (short)E_Master.OrderType);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region OrderType CRUD

        [HttpGet]
        public async Task<JsonResult> OrderTypeList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _orderTypeService.GetOrderTypeListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order type list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetOrderTypeById(short orderTypeId, string companyId)
        {
            if (orderTypeId <= 0)
                return Json(new { success = false, message = "Invalid Order Type ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _orderTypeService.GetOrderTypeByIdAsync(companyIdShort, parsedUserId.Value, orderTypeId);
                return data == null
                    ? Json(new { success = false, message = "Order Type not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order type by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrderType([FromBody] SaveOrderTypeViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var orderTypeToSave = new M_OrderType
                {
                    OrderTypeId = model.orderType.OrderTypeId,
                    CompanyId = companyIdShort,
                    OrderTypeCode = model.orderType.OrderTypeCode ?? string.Empty,
                    OrderTypeName = model.orderType.OrderTypeName ?? string.Empty,
                    OrderTypeCategoryId = model.orderType.OrderTypeCategoryId,
                    Remarks = model.orderType.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.orderType.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = model.orderType.EditById ?? 0,
                    EditDate = DateTime.UtcNow
                };

                var result = await _orderTypeService.SaveOrderTypeAsync(companyIdShort, parsedUserId.Value, orderTypeToSave);
                return Json(new { success = true, message = "Order Type saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving order type");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrderType(short orderTypeId, string companyId)
        {
            if (orderTypeId <= 0)
                return Json(new { success = false, message = "Invalid Order Type ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.OrderType);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var orderType = await _orderTypeService.GetOrderTypeByIdAsync(companyIdShort, parsedUserId.Value, orderTypeId);
                if (orderType == null)
                    return Json(new { success = false, message = "Order Type not found" });

                await _orderTypeService.DeleteOrderTypeAsync(companyIdShort, parsedUserId.Value, orderType);
                return Json(new { success = true, message = "Order Type deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order type");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion OrderType CRUD

        #region OrderTypeCategory CRUD

        [HttpGet]
        public async Task<JsonResult> OrderTypeCategoryList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _orderTypeService.GetOrderTypeCategoryListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order type category list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetOrderTypeCategoryById(short orderTypeCategoryId, string companyId)
        {
            if (orderTypeCategoryId <= 0)
                return Json(new { success = false, message = "Invalid Order Type Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _orderTypeService.GetOrderTypeCategoryByIdAsync(companyIdShort, parsedUserId.Value, orderTypeCategoryId);
                return data == null
                    ? Json(new { success = false, message = "Order Type Category not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order type category by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrderTypeCategory([FromBody] SaveOrderTypeCategoryViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var categoryToSave = new M_OrderTypeCategory
                {
                    OrderTypeCategoryId = model.orderTypeCategory.OrderTypeCategoryId,
                    CompanyId = companyIdShort,
                    OrderTypeCategoryCode = model.orderTypeCategory.OrderTypeCategoryCode ?? string.Empty,
                    OrderTypeCategoryName = model.orderTypeCategory.OrderTypeCategoryName ?? string.Empty,
                    Remarks = model.orderTypeCategory.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.orderTypeCategory.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = model.orderTypeCategory.EditById ?? 0,
                    EditDate = DateTime.UtcNow
                };

                var result = await _orderTypeService.SaveOrderTypeCategoryAsync(companyIdShort, parsedUserId.Value, categoryToSave);
                return Json(new { success = true, message = "Order Type Category saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving order type category");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrderTypeCategory(short orderTypeCategoryId, string companyId)
        {
            if (orderTypeCategoryId <= 0)
                return Json(new { success = false, message = "Invalid Order Type Category ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.OrderTypeCategory);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var category = await _orderTypeService.GetOrderTypeCategoryByIdAsync(companyIdShort, parsedUserId.Value, orderTypeCategoryId);
                if (category == null)
                    return Json(new { success = false, message = "Order Type Category not found" });

                await _orderTypeService.DeleteOrderTypeCategoryAsync(companyIdShort, parsedUserId.Value, category);
                return Json(new { success = true, message = "Order Type Category deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order type category");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion OrderTypeCategory CRUD
    }
}