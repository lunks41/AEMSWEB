﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class AllLogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}