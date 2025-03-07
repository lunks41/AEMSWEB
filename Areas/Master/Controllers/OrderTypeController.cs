//using AEMSWEB.Controllers;
//using AEMSWEB.Models.Masters;
//using AEMSWEB.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AEMSWEB.Areas.Master.Controllers
//{
//    [Area("master")]
//    public class OrderTypeController : BaseController
//    {
//        private readonly ILogger<OrderTypeController> _logger;

//        public OrderTypeController(
//            ILogger<OrderTypeController> logger

//           )

//        {
//            _logger = logger;
//        }

//        // GET: /master/OrderType/Index
//        public async Task<IActionResult> Index()
//        {
//            return View();
//        }

//        // GET: /master/OrderType/List
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

//                var apiResponse = await _apiService.GetAsync<List<OrderTypeViewModel>>("/master/getordertypes", headers);
//                return Json(apiResponse.Data);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while fetching order types.");
//                return Json(null);
//            }
//        }

//        // GET: /master/OrderType/GetById
//        [HttpGet]
//        public async Task<JsonResult> GetById(short orderTypeId, string companyId)
//        {
//            if (orderTypeId <= 0)
//            {
//                return Json(new { success = false, message = "Invalid Order Type ID." });
//            }

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.GetAsync<OrderTypeViewModel>($"/master/getordertypebyid/{orderTypeId}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, data = apiResponse.Data });
//            }
//            else
//            {
//                return Json(new { success = false, message = "Order Type not found." });
//            }
//        }

//        // POST: /master/OrderType/Save
//        [HttpPost]
//        public async Task<IActionResult> Save([FromBody] SaveOrderTypeViewModel model)
//        {
//            if (model == null)
//            {
//                return BadRequest(new { success = false, message = "Data operation failed." });
//            }

//            var orderType = model.OrderType;
//            var companyId = model.CompanyId;

//            var orderTypeToSave = new OrderTypeViewModel
//            {
//                CompanyId = Convert.ToInt16(companyId),
//                OrderTypeId = orderType.OrderTypeId,
//                OrderTypeCode = orderType.OrderTypeCode ?? string.Empty,
//                OrderTypeName = orderType.OrderTypeName ?? string.Empty,
//                OrderTypeCategoryId = orderType.OrderTypeCategoryId,
//                OrderTypeCategoryCode = orderType.OrderTypeCategoryCode ?? string.Empty,
//                OrderTypeCategoryName = orderType.OrderTypeCategoryName ?? string.Empty,
//                Remarks = orderType.Remarks?.Trim() ?? string.Empty,
//                IsActive = orderType.IsActive,
//                CreateById = orderType.CreateById,
//                CreateDate = DateTime.Now,
//                EditById = orderType.EditById ?? 0,
//                EditDate = DateTime.Now,
//                CreateBy = orderType.CreateBy ?? string.Empty,
//                EditBy = orderType.EditBy ?? string.Empty
//            };

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.PostAsync<OrderTypeViewModel>("/master/saveordertype", orderTypeToSave, headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record saved successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Save operation failed." });
//        }

//        // DELETE: /master/OrderType/Delete
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

//            var apiResponse = await _apiService.DeleteAsync($"/master/deleteordertype/{id}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record deleted successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Delete operation failed." });
//        }
//    }
//}