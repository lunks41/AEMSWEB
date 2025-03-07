//using AEMSWEB.Controllers;
//using AEMSWEB.Models.Masters;
//using AEMSWEB.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AEMSWEB.Areas.Master.Controllers
//{
//    [Area("master")]
//    public class PaymentTypeController : BaseController
//    {
//        private readonly ILogger<PaymentTypeController> _logger;

//        public PaymentTypeController(
//            ILogger<PaymentTypeController> logger

//           )

//        {
//            _logger = logger;
//        }

//        // GET: /master/PaymentType/Index
//        public async Task<IActionResult> Index()
//        {
//            return View();
//        }

//        // GET: /master/PaymentType/List
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

//                var apiResponse = await _apiService.GetAsync<List<PaymentTypeViewModel>>("/master/getpaymenttypes", headers);
//                return Json(apiResponse.Data);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while fetching payment types.");
//                return Json(null);
//            }
//        }

//        // GET: /master/PaymentType/GetById
//        [HttpGet]
//        public async Task<JsonResult> GetById(short paymentTypeId, string companyId)
//        {
//            if (paymentTypeId <= 0)
//            {
//                return Json(new { success = false, message = "Invalid Payment Type ID." });
//            }

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.GetAsync<PaymentTypeViewModel>($"/master/getpaymenttypebyid/{paymentTypeId}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, data = apiResponse.Data });
//            }
//            else
//            {
//                return Json(new { success = false, message = "Payment Type not found." });
//            }
//        }

//        // POST: /master/PaymentType/Save
//        [HttpPost]
//        public async Task<IActionResult> Save([FromBody] SavePaymentTypeViewModel model)
//        {
//            if (model == null)
//            {
//                return BadRequest(new { success = false, message = "Data operation failed." });
//            }

//            var paymentType = model.PaymentType;
//            var companyId = model.CompanyId;

//            var paymentTypeToSave = new PaymentTypeViewModel
//            {
//                PaymentTypeId = paymentType.PaymentTypeId,
//                CompanyId = Convert.ToInt16(companyId),
//                PaymentTypeCode = paymentType.PaymentTypeCode ?? string.Empty,
//                PaymentTypeName = paymentType.PaymentTypeName ?? string.Empty,
//                Remarks = paymentType.Remarks?.Trim() ?? string.Empty,
//                IsActive = paymentType.IsActive,
//                CreateById = paymentType.CreateById ?? 0,
//                CreateDate = DateTime.Now,
//                EditById = paymentType.EditById ?? 0,
//                EditDate = DateTime.Now,
//                CreateBy = paymentType.CreateBy ?? string.Empty,
//                EditBy = paymentType.EditBy ?? string.Empty
//            };

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.PostAsync<PaymentTypeViewModel>("/master/savepaymenttype", paymentTypeToSave, headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record saved successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Save operation failed." });
//        }

//        // DELETE: /master/PaymentType/Delete
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

//            var apiResponse = await _apiService.DeleteAsync($"/master/deletepaymenttype/{id}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record deleted successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Delete operation failed." });
//        }
//    }
//}