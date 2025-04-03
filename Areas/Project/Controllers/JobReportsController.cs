using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Areas.Project.Controllers
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