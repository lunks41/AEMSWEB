using AEMSWEB.Models;
using AEMSWEB.Repository;
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

        // Assume _repository is injected via constructor in your actual implementation
        protected readonly IRepository<TransactionViewModel> _repository; // Add your repository interface

        // Define TransactionCode as an abstract property that derived controllers must implement
        protected abstract string TransactionCode { get; }

        public BaseController(ILogger<BaseController> logger, IRepository<TransactionViewModel> repository)
        {
            _logger = logger;
            _repository = repository;
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

            // Optional: Verify route companyCode matches session
            var routeCompanyCode = context.RouteData.Values["companyId"]?.ToString();
            var sessionCompanyCode = HttpContext.Session.GetString("CurrentCompany");
            if (!string.IsNullOrEmpty(routeCompanyCode) && routeCompanyCode != sessionCompanyCode)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            //// Fetch sidebar and permission data
            //var modulesResponse = await _repository.GetQueryAsync<TransactionViewModel>(
            //    $"exec Adm_GetUserTransactions_All {CompanyId}, '1'"
            //);

            //if (modulesResponse.Count() > 0)
            //{
            //    var moduleViews = modulesResponse
            //        .GroupBy(t => new { t.ModuleId, t.ModuleCode, t.ModuleName })
            //        .Select(g => new ModuleView
            //        {
            //            ModuleId = g.Key.ModuleId,
            //            ModuleCode = g.Key.ModuleCode,
            //            ModuleName = g.Key.ModuleName,
            //            TransCategorys = g.GroupBy(tc => new { tc.TransCategoryId, tc.TransCategoryCode, tc.TransCategoryName })
            //                .Select(tcg => new TransCategoryView
            //                {
            //                    TransCategoryId = tcg.Key.TransCategoryId,
            //                    TransCategoryCode = tcg.Key.TransCategoryCode,
            //                    TransCategoryName = tcg.Key.TransCategoryName,
            //                    Transactions = tcg.Select(t => new TransactionView
            //                    {
            //                        TransactionId = t.TransactionId,
            //                        TransactionCode = t.TransactionCode,
            //                        TransactionName = t.TransactionName,
            //                        IsCreate = t.IsCreate,
            //                        IsEdit = t.IsEdit,
            //                        IsDelete = t.IsDelete,
            //                        IsExport = t.IsExport,
            //                        IsPrint = t.IsPrint
            //                    }).ToList()
            //                }).ToList()
            //        }).ToList();

            //    ViewBag.Modules = moduleViews;

            //    // Check permissions using transaction data
            //    if (!string.IsNullOrEmpty(TransactionCode))
            //    {
            //        var allTransactions = moduleViews
            //            .SelectMany(m => m.TransCategorys.SelectMany(tc => tc.Transactions))
            //            .ToList();
            //        var transaction = allTransactions.FirstOrDefault(t => t.TransactionCode.ToLower() == TransactionCode.ToLower());

            //        if (transaction == null)
            //        {
            //            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            //            return;
            //        }

            //        var actionName = context.ActionDescriptor.RouteValues["action"];
            //        var requiredPermission = GetRequiredPermission(actionName);
            //        bool hasPermission = requiredPermission switch
            //        {
            //            "IsView" => true, // View access if transaction exists
            //            "IsCreate" => transaction.IsCreate,
            //            "IsEdit" => transaction.IsEdit,
            //            "IsDelete" => transaction.IsDelete,
            //            "IsPrint" => transaction.IsPrint,
            //            _ => false
            //        };

            //        if (!hasPermission)
            //        {
            //            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            //            return;
            //        }
            //    }
            //}
            //else
            //{
            //    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            //    return;
            //}

            base.OnActionExecuting(context);
        }

        protected string GetRequiredPermission(string actionName)
        {
            return actionName switch
            {
                "Index" => "IsView",
                "Create" => "IsCreate",
                "Edit" => "IsEdit",
                "Delete" => "IsDelete",
                "Print" => "IsPrint",
                _ => "IsView" // Default to view permission
            };
        }
    }
}

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System.Security.Claims;
//using AEMSWEB.Extension;

//namespace AEMSWEB.Controllers
//{
//    public class BaseController : Controller
//    {
//        protected Guid CompanyId { get; private set; }
//        protected string UserId { get; private set; }

//        public override void OnActionExecuting(ActionExecutingContext context)
//        {
//            // Get company code from route
//            var companyCode = context.RouteData.Values["companyCode"]?.ToString();
//            CompanyId = ConvertCompanyCodeToId(companyCode);

//            // Get user ID from claims
//            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//            // Get action name correctly
//            var actionName = context.ActionDescriptor.RouteValues["action"];

//            // Verify permissions
//            if (!CheckPermissions(actionName))
//            {
//                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
//            }

//            base.OnActionExecuting(context);
//        }

//        private Guid ConvertCompanyCodeToId(string companyCode)
//        {
//            // Simplified example - implement your actual mapping logic
//            return Guid.TryParse(companyCode, out var guid) ? guid : Guid.Empty;
//        }

//        private bool CheckPermissions(string actionName)
//        {
//            var requiredPermission = GetRequiredPermission(actionName);
//            var permissions = HttpContext.Session.GetObject<Dictionary<string, bool>>("Permissions");
//            return permissions?.ContainsKey(requiredPermission) == true && permissions[requiredPermission];
//        }

//        protected string GetRequiredPermission(string actionName)
//        {
//            return actionName switch
//            {
//                "Index" => "IsView",
//                "Create" => "IsCreate",
//                "Edit" => "IsEdit",
//                "Delete" => "IsDelete",
//                "Print" => "IsPrint",
//                _ => "IsView" // Default permission
//            };
//        }
//    }
//}