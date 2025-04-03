using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}