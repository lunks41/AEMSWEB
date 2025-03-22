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
    public class CreditTermsController : BaseController
    {
        private readonly ILogger<CreditTermsController> _logger;
        private readonly ICreditTermsService _creditTermsService;

        public CreditTermsController(ILogger<CreditTermsController> logger, IBaseService baseService, ICreditTermsService creditTermsService)
            : base(logger, baseService)
        {
            _logger = logger;
            _creditTermsService = creditTermsService;
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
                (short)E_Modules.Master, (short)E_Master.CreditTerms);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region CreditTerms CRUD

        [HttpGet]
        public async Task<JsonResult> CreditTermList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _creditTermsService.GetCreditTermListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching credit term list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCreditTermById(short creditTermId, string companyId)
        {
            if (creditTermId <= 0)
                return Json(new { success = false, message = "Invalid Credit Term ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _creditTermsService.GetCreditTermByIdAsync(companyIdShort, parsedUserId.Value, creditTermId);
                return data == null
                    ? Json(new { success = false, message = "Credit Term not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching credit term by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCreditTerm([FromBody] SaveCreditTermViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var creditTermToSave = new M_CreditTerm
                {
                    CreditTermId = model.creditTerm.CreditTermId,
                    CompanyId = companyIdShort,
                    CreditTermCode = model.creditTerm.CreditTermCode ?? string.Empty,
                    CreditTermName = model.creditTerm.CreditTermName ?? string.Empty,
                    NoDays = model.creditTerm.NoDays,
                    Remarks = model.creditTerm.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.creditTerm.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _creditTermsService.SaveCreditTermAsync(companyIdShort, parsedUserId.Value, creditTermToSave);
                return Json(new { success = true, message = "Credit Term saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving credit term");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCreditTerm(short creditTermId, string companyId)
        {
            if (creditTermId <= 0)
                return Json(new { success = false, message = "Invalid Credit Term ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.CreditTerms);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _creditTermsService.DeleteCreditTermAsync(companyIdShort, parsedUserId.Value, creditTermId);
                return Json(new { success = true, message = "Credit Term deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting credit term");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion CreditTerms CRUD

        #region CreditTermsDt CRUD

        [HttpGet]
        public async Task<JsonResult> CreditTermDtList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _creditTermsService.GetCreditTermDtListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching credit term details list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCreditTermDtById(short creditTermDtId, short fromDay, string companyId)
        {
            if (creditTermDtId <= 0)
                return Json(new { success = false, message = "Invalid Credit Term Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _creditTermsService.GetCreditTermDtByIdAsync(companyIdShort, parsedUserId.Value, creditTermDtId, fromDay);
                return data == null
                    ? Json(new { success = false, message = "Credit Term Detail not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching credit term detail by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCreditTermDt([FromBody] SaveCreditTermDtViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var detailToSave = new M_CreditTermDt
                {
                    CreditTermId = model.creditTermDt.CreditTermId,
                    CompanyId = companyIdShort,
                    FromDay = model.creditTermDt.FromDay,
                    ToDay = model.creditTermDt.ToDay,
                    IsEndOfMonth = model.creditTermDt.IsEndOfMonth,
                    DueDay = model.creditTermDt.DueDay,
                    NoMonth = model.creditTermDt.NoMonth,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _creditTermsService.SaveCreditTermDtAsync(companyIdShort, parsedUserId.Value, detailToSave);
                return Json(new { success = true, message = "Credit Term Detail saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving credit term detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCreditTermDt(short creditTermDtId, short fromDay, string companyId)
        {
            if (creditTermDtId <= 0)
                return Json(new { success = false, message = "Invalid Credit Term Detail ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.CreditTermDt);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _creditTermsService.DeleteCreditTermDtAsync(companyIdShort, parsedUserId.Value, creditTermDtId, fromDay);
                return Json(new { success = true, message = "Credit Term Detail deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting credit term detail");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion CreditTermsDt CRUD
    }
}