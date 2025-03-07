using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Project.Controllers
{
    [Area("project")]
    public class JobReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}