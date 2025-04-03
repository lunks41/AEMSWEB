using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Areas.Project.Controllers
{
    [Area("project")]
    public class QuotationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}