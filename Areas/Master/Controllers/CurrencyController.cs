using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Controllers;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Models.Masters;
using AEMSWEB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class CurrencyController : Controller
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ILogger<CurrencyController> logger, ICurrencyService currencyService)
        {
            _logger = logger;
            _currencyService = currencyService;
        }

        // GET: /master/Currency/Index
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

                var data = await _currencyService.GetCurrencyListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching currencies.");
                return Json(new { Result = -1, Message = "An error occurred" });
            }
        }

        // GET: /master/Currency/GetById
        [HttpGet]
        public async Task<JsonResult> GetById(short currencyId, string companyId)
        {
            if (currencyId <= 0)
            {
                return Json(new { success = false, message = "Invalid Currency ID." });
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
                var data = await _currencyService.GetCurrencyByIdAsync(companyIdShort, parsedUserId, currencyId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Currency not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching currency by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: /master/Currency/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveCurrencyViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var currency = model.Currency;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var currencyToSave = new M_Currency
            {
                CurrencyId = currency.CurrencyId,
                CompanyId = companyIdShort,
                CurrencyCode = currency.CurrencyCode ?? string.Empty,
                CurrencyName = currency.CurrencyName ?? string.Empty,
                IsMultiply = currency.IsMultiply,
                Remarks = currency.Remarks?.Trim() ?? string.Empty,
                IsActive = currency.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = currency.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _currencyService.SaveCurrencyAsync(companyIdShort, parsedUserId, currencyToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save currency." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the currency.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        // DELETE: /master/Currency/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(short currencyId, string companyId)
        {
            if (currencyId <= 0)
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
                var currencyGet = await _currencyService.GetCurrencyByIdAsync(companyIdShort, parsedUserId, currencyId);

                var data = await _currencyService.DeleteCurrencyAsync(companyIdShort, 1, currencyId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save currency." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the currency.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }
    }
}