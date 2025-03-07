using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Project.Controllers
{
    //Rates of customer task wise
    [Area("project")]
    public class TariffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}