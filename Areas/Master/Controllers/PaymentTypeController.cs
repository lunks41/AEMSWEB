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
    public class PaymentTypeController : BaseController
    {
        private readonly ILogger<PaymentTypeController> _logger;
        private readonly IPaymentTypeService _paymentTypeService;

        public PaymentTypeController(ILogger<PaymentTypeController> logger,
            IBaseService baseService,
            IPaymentTypeService paymentTypeService)
            : base(logger, baseService)
        {
            _logger = logger;
            _paymentTypeService = paymentTypeService;
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
                (short)E_Modules.Master, (short)E_Master.PaymentType);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region PaymentType CRUD

        [HttpGet]
        public async Task<JsonResult> PaymentTypeList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _paymentTypeService.GetPaymentTypeListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment type list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetPaymentTypeById(short paymentTypeId, string companyId)
        {
            if (paymentTypeId <= 0)
                return Json(new { success = false, message = "Invalid Payment Type ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _paymentTypeService.GetPaymentTypeByIdAsync(companyIdShort, parsedUserId.Value, paymentTypeId);
                return data == null
                    ? Json(new { success = false, message = "Payment Type not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment type by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePaymentType([FromBody] SavePaymentTypeViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var paymentTypeToSave = new M_PaymentType
                {
                    PaymentTypeId = model.paymentType.PaymentTypeId,
                    CompanyId = companyIdShort,
                    PaymentTypeCode = model.paymentType.PaymentTypeCode ?? string.Empty,
                    PaymentTypeName = model.paymentType.PaymentTypeName ?? string.Empty,
                    Remarks = model.paymentType.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.paymentType.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = model.paymentType.EditById ?? 0,
                    EditDate = DateTime.UtcNow
                };

                var result = await _paymentTypeService.SavePaymentTypeAsync(companyIdShort, parsedUserId.Value, paymentTypeToSave);
                return Json(new { success = true, message = "Payment Type saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving payment type");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePaymentType(short paymentTypeId, string companyId)
        {
            if (paymentTypeId <= 0)
                return Json(new { success = false, message = "Invalid Payment Type ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.PaymentType);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _paymentTypeService.DeletePaymentTypeAsync(companyIdShort, parsedUserId.Value, paymentTypeId);
                return Json(new { success = true, message = "Payment Type deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment type");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion PaymentType CRUD
    }
}