//using AEMSWEB.Controllers;
//using AEMSWEB.Models.Masters;
//using AEMSWEB.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AEMSWEB.Areas.Master.Controllers
//{
//    [Area("master")]
//    public class DesignationController : BaseController
//    {
//        private readonly ILogger<DesignationController> _logger;

//        public DesignationController(
//            ILogger<DesignationController> logger

//           )

//        {
//            _logger = logger;
//        }

//        // GET: /master/Designation/Index
//        public async Task<IActionResult> Index()
//        {
//            return View();
//        }

//        // GET: /master/Designation/List
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

//                var apiResponse = await _apiService.GetAsync<List<DesignationViewModel>>("/master/getdesignations", headers);
//                return Json(apiResponse.Data);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while fetching designations.");
//                return Json(null);
//            }
//        }

//        // GET: /master/Designation/GetById
//        [HttpGet]
//        public async Task<JsonResult> GetById(short designationId, string companyId)
//        {
//            if (designationId <= 0)
//            {
//                return Json(new { success = false, message = "Invalid Designation ID." });
//            }

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.GetAsync<DesignationViewModel>($"/master/getdesignationbyid/{designationId}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, data = apiResponse.Data });
//            }
//            else
//            {
//                return Json(new { success = false, message = "Designation not found." });
//            }
//        }

//        // POST: /master/Designation/Save
//        [HttpPost]
//        public async Task<IActionResult> Save([FromBody] SaveDesignationViewModel model)
//        {
//            if (model == null)
//            {
//                return BadRequest(new { success = false, message = "Data operation failed." });
//            }

//            var designation = model.Designation;
//            var companyId = model.CompanyId;

//            var designationToSave = new DesignationViewModel
//            {
//                DesignationId = designation.DesignationId,
//                CompanyId = Convert.ToInt16(companyId),
//                DesignationCode = designation.DesignationCode ?? string.Empty,
//                DesignationName = designation.DesignationName ?? string.Empty,
//                Remarks = designation.Remarks?.Trim() ?? string.Empty,
//                IsActive = designation.IsActive,
//                CreateById = designation.CreateById,
//                CreateDate = DateTime.Now,
//                EditById = designation.EditById ?? 0,
//                EditDate = DateTime.Now,
//                CreateBy = designation.CreateBy ?? string.Empty,
//                EditBy = designation.EditBy ?? string.Empty
//            };

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.PostAsync<DesignationViewModel>("/master/savedesignation", designationToSave, headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record saved successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Save operation failed." });
//        }

//        // DELETE: /master/Designation/Delete
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

//            var apiResponse = await _apiService.DeleteAsync($"/master/deletedesignation/{id}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record deleted successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Delete operation failed." });
//        }
//    }
//}