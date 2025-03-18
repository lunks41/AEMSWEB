using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class CreditTermController : Controller
    {
        private readonly ILogger<CreditTermController> _logger;
        private readonly ICreditTermService _countryService;

        public CreditTermController(ILogger<CreditTermController> logger, ICreditTermService countryService)
        {
            _logger = logger;
            _countryService = countryService;
        }

        // GET: /master/CreditTerm/Index
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("List")]
        public async Task<JsonResult> List(short pageNumber, short pageSize, string searchString, string companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
                {
                    return Json(new { Result = -1, Message = "Invalid company ID" });
                }

                var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var data = await _countryService.GetCreditTermListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching countries.");
                return Json(new { Result = -1, Message = "An error occurred" });
            }
        }

        // GET: /master/CreditTerm/GetById
        [HttpGet]
        public async Task<JsonResult> GetById(short countryId, string companyId)
        {
            if (countryId <= 0)
            {
                return Json(new { success = false, message = "Invalid CreditTerm ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                var data = await _countryService.GetCreditTermByIdAsync(companyIdShort, parsedUserId, countryId);

                if (data == null)
                {
                    return Json(new { success = false, message = "CreditTerm not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching country by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: /master/CreditTerm/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveCreditTermViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var country = model.CreditTerm;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var countryToSave = new M_CreditTerm
            {
                CreditTermId = country.CreditTermId,
                CompanyId = companyIdShort,
                CreditTermCode = country.CreditTermCode ?? string.Empty,
                CreditTermName = country.CreditTermName ?? string.Empty,
                Remarks = country.Remarks?.Trim() ?? string.Empty,
                IsActive = country.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = country.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _countryService.SaveCreditTermAsync(companyIdShort, parsedUserId, countryToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save country." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the country.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        // DELETE: /master/CreditTerm/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(short countryId, string companyId)
        {
            if (countryId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                var countryGet = await _countryService.GetCreditTermByIdAsync(companyIdShort, parsedUserId, countryId);

                var data = await _countryService.DeleteCreditTermAsync(companyIdShort, 1, countryGet);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save country." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the country.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }
    }
}