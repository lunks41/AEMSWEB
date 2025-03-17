using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Controllers
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