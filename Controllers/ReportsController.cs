using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}