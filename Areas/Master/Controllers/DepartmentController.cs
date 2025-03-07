//using AEMSWEB.Controllers;
//using AEMSWEB.Models.Masters;
//using AEMSWEB.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AEMSWEB.Areas.Master.Controllers
//{
//    [Area("master")]
//    public class DepartmentController : BaseController
//    {
//        private readonly ILogger<DepartmentController> _logger;

//        public DepartmentController(
//            ILogger<DepartmentController> logger

//           )

//        {
//            _logger = logger;
//        }

//        // GET: /master/Department/Index
//        public async Task<IActionResult> Index()
//        {
//            return View();
//        }

//        // GET: /master/Department/List
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

//                var apiResponse = await _apiService.GetAsync<List<DepartmentViewModel>>("/master/getdepartments", headers);
//                return Json(apiResponse.Data);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while fetching departments.");
//                return Json(null);
//            }
//        }

//        // GET: /master/Department/GetById
//        [HttpGet]
//        public async Task<JsonResult> GetById(short departmentId, string companyId)
//        {
//            if (departmentId <= 0)
//            {
//                return Json(new { success = false, message = "Invalid Department ID." });
//            }

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.GetAsync<DepartmentViewModel>($"/master/getdepartmentbyid/{departmentId}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, data = apiResponse.Data });
//            }
//            else
//            {
//                return Json(new { success = false, message = "Department not found." });
//            }
//        }

//        // POST: /master/Department/Save
//        [HttpPost]
//        public async Task<IActionResult> Save([FromBody] SaveDepartmentViewModel model)
//        {
//            if (model == null)
//            {
//                return BadRequest(new { success = false, message = "Data operation failed." });
//            }

//            var department = model.Department;
//            var companyId = model.CompanyId;

//            var departmentToSave = new DepartmentViewModel
//            {
//                DepartmentId = department.DepartmentId,
//                CompanyId = Convert.ToInt16(companyId),
//                DepartmentCode = department.DepartmentCode ?? string.Empty,
//                DepartmentName = department.DepartmentName ?? string.Empty,
//                Remarks = department.Remarks?.Trim() ?? string.Empty,
//                IsActive = department.IsActive,
//                CreateById = department.CreateById,
//                CreateDate = DateTime.Now,
//                EditById = department.EditById ?? 0,
//                EditDate = DateTime.Now,
//                CreateBy = department.CreateBy ?? string.Empty,
//                EditBy = department.EditBy ?? string.Empty
//            };

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.PostAsync<DepartmentViewModel>("/master/savedepartment", departmentToSave, headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record saved successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Save operation failed." });
//        }

//        // DELETE: /master/Department/Delete
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

//            var apiResponse = await _apiService.DeleteAsync($"/master/deletedepartment/{id}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record deleted successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Delete operation failed." });
//        }
//    }
//}