//using AEMSWEB.Controllers;
//using AEMSWEB.Models.Masters;
//using AEMSWEB.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AEMSWEB.Areas.Master.Controllers
//{
//    [Area("master")]
//    public class ProductController : BaseController
//    {
//        private readonly ILogger<ProductController> _logger;

//        public ProductController(
//            ILogger<ProductController> logger

//           )

//        {
//            _logger = logger;
//        }

//        // GET: /master/Product/Index
//        public async Task<IActionResult> Index()
//        {
//            return View();
//        }

//        // GET: /master/Product/List
//        [HttpGet]
//        public async Task<JsonResult> List(string searchString, string companyId)
//        {
//            try
//            {
//                var headers = new Dictionary<string, string>
//            {
//                { "PageSize", "10" },
//                { "PageNumber", "1" },
//                { "SearchString", searchString ?? string.Empty }
//            };

//                if (!string.IsNullOrEmpty(companyId))
//                {
//                    headers.Add("CompanyId", companyId);
//                }

//                var apiResponse = await _apiService.GetAsync<List<ProductViewModel>>("/master/getproducts", headers);
//                return Json(apiResponse.Data);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while fetching products.");
//                return Json(null);
//            }
//        }

//        // GET: /master/Product/GetById
//        [HttpGet]
//        public async Task<JsonResult> GetById(short productId, string companyId)
//        {
//            if (productId <= 0)
//            {
//                return Json(new { success = false, message = "Invalid Product ID." });
//            }

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.GetAsync<ProductViewModel>($"/master/getproductbyid/{productId}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, data = apiResponse.Data });
//            }
//            else
//            {
//                return Json(new { success = false, message = "Product not found." });
//            }
//        }

//        // POST: /master/Product/Save
//        [HttpPost]
//        public async Task<IActionResult> Save([FromBody] SaveProductViewModel model)
//        {
//            if (model == null)
//            {
//                return BadRequest(new { success = false, message = "Data operation failed." });
//            }

//            var product = model.Product;
//            var companyId = model.CompanyId;

//            var productToSave = new ProductViewModel
//            {
//                ProductId = product.ProductId,
//                CompanyId = Convert.ToInt16(companyId),
//                ProductCode = product.ProductCode ?? string.Empty,
//                ProductName = product.ProductName ?? string.Empty,
//                Remarks = product.Remarks?.Trim() ?? string.Empty,
//                IsActive = product.IsActive,
//                CreateById = product.CreateById ?? 0,
//                CreateDate = DateTime.Now,
//                EditById = product.EditById ?? 0,
//                EditDate = DateTime.Now,
//                CreateBy = product.CreateBy ?? string.Empty,
//                EditBy = product.EditBy ?? string.Empty
//            };

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.PostAsync<ProductViewModel>("/master/saveproduct", productToSave, headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record saved successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Save operation failed." });
//        }

//        // DELETE: /master/Product/Delete
//        [HttpDelete]
//        public async Task<IActionResult> Delete(int id, string companyId)
//        {
//            if (id <= 0)
//            {
//                return BadRequest(new { success = false, message = "Invalid ID." });
//            }

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.DeleteAsync($"/master/deleteproduct/{id}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record deleted successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Delete operation failed." });
//        }
//    }
//}