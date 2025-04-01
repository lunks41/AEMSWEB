using AEMSWEB.IServices;
using AEMSWEB.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AEMSWEB.Controllers
{
    public abstract class BaseController : Controller
    {
        protected byte CompanyId { get; private set; }
        protected string UserId { get; private set; }
        protected readonly ILogger<BaseController> _logger;
        private readonly IBaseService _baseService;

        public BaseController(ILogger<BaseController> logger, IBaseService baseService)
        {
            _logger = logger;
            _baseService = baseService;
        }

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            // Get company ID from session
            var currentCompanyId = HttpContext.Session.GetString("CurrentCompany");
            if (string.IsNullOrEmpty(currentCompanyId) || !byte.TryParse(currentCompanyId, out var companyId))
            {
                context.Result = new RedirectToActionResult("SelectCompany", "Account", null);
                return;
            }
            CompanyId = companyId;

            // Get user ID from claims
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserId))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            ////checking urlcompanyId & session companyId
            //// Optional: Verify route companyCode matches session
            //var routeCompanyCode = context.RouteData.Values["companyId"]?.ToString();
            //var sessionCompanyCode = HttpContext.Session.GetString("CurrentCompany");
            //if (!string.IsNullOrEmpty(routeCompanyCode) && routeCompanyCode != sessionCompanyCode)
            //{
            //    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            //    return;
            //}

            base.OnActionExecuting(context);
        }

        protected async Task<UserGroupRightsViewModel> HasPermission(Int16 companyId, Int32 userId, Int16 moduleId, Int16 transactionId)
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            return await _baseService.ValidateScreen(companyId, userId, moduleId, transactionId);
        }

        protected short? GetParsedUserId()
        {
            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId) && short.TryParse(userId, out var parsedUserId))
            {
                return parsedUserId;
            }
            return null;
        }

        protected JsonResult ValidateCompanyAndUserId(string companyId, out byte companyIdByte, out short? parsedUserId)
        {
            companyIdByte = 0;
            parsedUserId = GetParsedUserId();

            if (string.IsNullOrEmpty(companyId) || !byte.TryParse(companyId, out companyIdByte))
            {
                _logger.LogWarning("Invalid company ID: {CompanyId}", companyId);
                return Json(new { success = false, message = "Invalid company ID." });
            }

            if (!parsedUserId.HasValue)
            {
                _logger.LogWarning("User not logged in or invalid user ID.");
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            return null;
        }
    }
}