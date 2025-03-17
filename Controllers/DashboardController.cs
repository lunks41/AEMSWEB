using AEMSWEB.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger, IBaseService baseService)
            : base(logger, baseService)
        {
            _logger = logger;
        }

        public IActionResult Finance()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}