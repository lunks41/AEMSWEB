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
    public class TaxController : Controller
    {
        private readonly ILogger<TaxController> _logger;
        private readonly ITaxService _taxService;

        public TaxController(ILogger<TaxController> logger, ITaxService taxService)
        {
            _logger = logger;
            _taxService = taxService;
        }

        // GET: /master/Tax/Index
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

                var data = await _taxService.GetTaxListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching taxes.");
                return Json(new { Result = -1, Message = "An error occurred" });
            }
        }

        // GET: /master/Tax/GetById
        [HttpGet]
        public async Task<JsonResult> GetById(short taxId, string companyId)
        {
            if (taxId <= 0)
            {
                return Json(new { success = false, message = "Invalid Tax ID." });
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
                var data = await _taxService.GetTaxByIdAsync(companyIdShort, parsedUserId, taxId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Tax not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching tax by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: /master/Tax/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveTaxViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var tax = model.Tax;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var taxToSave = new M_Tax
            {
                TaxId = tax.TaxId,
                CompanyId = companyIdShort,
                TaxCategoryId = tax.TaxCategoryId,
                TaxCode = tax.TaxCode ?? string.Empty,
                TaxName = tax.TaxName ?? string.Empty,
                Remarks = tax.Remarks?.Trim() ?? string.Empty,
                IsActive = tax.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = tax.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _taxService.SaveTaxAsync(companyIdShort, parsedUserId, taxToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save tax." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the tax.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        // DELETE: /master/Tax/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(short taxId, string companyId)
        {
            if (taxId <= 0)
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
                var taxGet = await _taxService.GetTaxByIdAsync(companyIdShort, parsedUserId, taxId);

                var data = await _taxService.DeleteTaxAsync(companyIdShort, 1, taxGet);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save tax." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the tax.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }
    }
}