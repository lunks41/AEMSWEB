using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/Forbidden")]
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}