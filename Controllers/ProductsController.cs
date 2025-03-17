using AEMSWEB.Data;
using AEMSWEB.Helpers;
using AEMSWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [PermissionAuthorize(1, 101, "Read")]
        public IActionResult Index()
        {
            return View();
        }

        [PermissionAuthorize(1, 101, "Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "RequireEdit")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([FromBody] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CompanyId = "1";
                _context.Add(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        // Similar Update, Delete actions with [Authorize(Policy = "...")]
    }
}