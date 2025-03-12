using AEMSWEB.Controllers;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.IServices.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AEMSWEB.Areas.Admin.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly ICompanyService _CompanyService;

        public CompanyController(ILogger<CompanyController> logger, IBaseService baseService, ICompanyService CompanyService) : base(logger, baseService)
        {
            _logger = logger;
            _CompanyService = CompanyService;
        }

        public async Task<IActionResult> Index()
        {
            var companyId = HttpContext.Session.GetString("CurrentCompany");
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }
            var permissions = await HasPermission(companyIdShort, parsedUserId, (short)E_Modules.Master, (short)E_Admin.Company);

            ViewBag.IsRead = permissions.IsRead;
            ViewBag.IsCreate = permissions.IsCreate;
            ViewBag.IsEdit = permissions.IsEdit;
            ViewBag.IsDelete = permissions.IsDelete;
            ViewBag.IsExport = permissions.IsExport;
            ViewBag.IsPrint = permissions.IsPrint;

            return View();
        }

        [HttpGet("Company/List")]
        public async Task<JsonResult> List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
                {
                    return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
                }

                var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var data = await _CompanyService.GetCompanyListAsync(pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total = total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account groups.");
                return Json(new { Result = -1, Message = "An error occurred", Data = "" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(short accGroupId, string companyId)
        {
            if (accGroupId <= 0)
            {
                return Json(new { success = false, message = "Invalid Account Group ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                // Assuming you would have some logic here to use the headers in your service call
                var data = await _CompanyService.GetCompanyByIdAsync(accGroupId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Account Group not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account group by ID.");
                return Json(new { success = false, message = "An error occurred", data = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(CompanyViewModel model)
        {
            //if (!await _permissionService.HasPermission(User.Identity.Name, "Company", "Edit"))
            //{
            //    return Forbid();
            //}

            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var companyToSave = new AdmCompany
            {
                CompanyId = model.CompanyId,
                CompanyCode = model.CompanyCode ?? string.Empty,
                CompanyName = model.CompanyName ?? string.Empty,
                RegistrationNo = model.RegistrationNo ?? string.Empty,
                TaxRegistrationNo = model.TaxRegistrationNo ?? string.Empty,
                Remarks = model.Remarks?.Trim() ?? string.Empty,
                IsActive = model.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = model.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _CompanyService.SaveCompanyAsync(parsedUserId, companyToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account group." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }

        //// DELETE: /master/Company/Delete
        //[HttpDelete]
        //public async Task<IActionResult> Delete(short accGroupId, string companyId)
        //{
        //    //if (!await _permissionService.HasPermission(User.Identity.Name, "Company", "Delete"))
        //    //{
        //    //    return Forbid();
        //    //}
        //    if (accGroupId <= 0)
        //    {
        //        return BadRequest(new { success = false, message = "Invalid ID." });
        //    }

        //    if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
        //    {
        //        return Json(new { success = false, message = "Invalid company ID." });
        //    }

        //    var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
        //    {
        //        return Json(new { success = false, message = "User not logged in or invalid user ID." });
        //    }

        //    try
        //    {
        //        var companyget = await _CompanyService.GetCompanyByIdAsync(companyIdShort, parsedUserId, accGroupId);

        //        var data = await _CompanyService.DeleteCompanyAsync(companyIdShort, 1, companyget);

        //        if (data == null)
        //        {
        //            return Json(new { success = false, message = "Failed to save account group." });
        //        }

        //        return Json(new { success = true, data });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while saving the account group.");
        //        return Json(new { success = false, message = "An error occurred.", data = "" });
        //    }
        //}
    }
}