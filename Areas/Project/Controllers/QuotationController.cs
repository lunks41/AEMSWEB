using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Project.Controllers
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