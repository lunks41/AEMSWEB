using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Setting.Controllers
{
    [Area("setting")]
    [Authorize]
    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}