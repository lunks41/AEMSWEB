//using AEMSWEB.Controllers;
//using AEMSWEB.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AEMSWEB.Areas.Master.Controllers
//{
//    [Area("master")]
//    public class PortController : BaseController
//    {
//        private readonly ILogger<PortController> _logger;

//        public PortController(
//                            ILogger<PortController> logger
//                             )

//        {
//            _logger = logger;
//        }

//        public IActionResult Index()
//        {
//            try
//            {
//                if (HttpContext.Session.GetInt32("selectedCompany") == null)
//                {
//                    return RedirectToAction("SelectCompany", "Company");
//                }

//                return View();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error in PortController.Index");
//                return StatusCode(500);
//            }
//        }

//        [HttpGet]
//        public IActionResult GetUsers()
//        {
//            var users = new List<object>
//        {
//            new { userId = 1, userName = "John Doe", userEmail = "john@example.com" },
//            new { userId = 2, userName = "Jane Doe", userEmail = "jane@example.com" }
//        };

//            return Json(users);
//        }

//        [HttpPost]
//        public JsonResult GetUsersV1()
//        {
//            // For demo purposes - hardcoded data
//            var data = new List<object>
//    {
//        new { userId = 1, userName = "John Doe", userEmail = "john@example.com" },
//        new { userId = 2, userName = "Jane Doe", userEmail = "jane@example.com" }
//    };

//            // DataTables expected response format
//            return Json(new
//            {
//                draw = 1,  // Echo back the draw parameter from the request
//                recordsTotal = data.Count,
//                recordsFiltered = data.Count,
//                data = data
//            });
//        }
//    }

//    //[Area("master")]
//    //public class PortController : BaseController
//    //{
//    //    private readonly ILogger<PortController> _logger;

//    //    public PortController( ILogger<PortController> logger IHttpContextAccessor httpContextAccessor) : base(logger  httpContextAccessor)
//    //    {
//    //        _logger = logger;
//    //    }

//    //    public IActionResult Index()
//    //    {
//    //        try
//    //        {
//    //            _logger.LogInformation("PortController - Index() method called.");
//    //            return View();
//    //        }
//    //        catch (Exception ex)
//    //        {
//    //            _logger.LogError(ex, "An error occurred in PortController - Index()");
//    //            return StatusCode(500, "Internal Server Error");
//    //        }
//    //    }

//    //    public IActionResult Save()
//    //    {
//    //        return View();
//    //    }

//    //    public IActionResult View()
//    //    {
//    //        return View();
//    //    }

//    //    public IActionResult Delete()
//    //    {
//    //        return View();
//    //    }
//    //}
//}