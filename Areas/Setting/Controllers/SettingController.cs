using AEMSWEB.Controllers;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.IServices.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AEMSWEB.Areas.Setting.Controllers
{
    [Area("setting")]
    [Authorize]
    public class SettingController : BaseController
    {
        private readonly ILogger<SettingController> _logger;
        private readonly IUserService _UserService;

        public SettingController(ILogger<SettingController> logger, IBaseService baseService, IUserService UserService) : base(logger, baseService)
        {
            _logger = logger;
            _UserService = UserService;
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
            var permissions = await HasPermission(companyIdShort, parsedUserId, (short)E_Modules.Setting, (short)E_Setting.UserSetting);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.IsExport = permissions?.IsExport ?? false;
            ViewBag.IsPrint = permissions?.IsPrint ?? false;

            return View();
        }
    }
}