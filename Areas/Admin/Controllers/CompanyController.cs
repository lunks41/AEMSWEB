using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Admin.Controllers
{
    public class CompanyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}