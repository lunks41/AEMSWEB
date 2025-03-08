using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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