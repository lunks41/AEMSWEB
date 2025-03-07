//using AEMSWEB.Controllers;
//using AEMSWEB.Models.Masters;
//using AEMSWEB.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AEMSWEB.Areas.Master.Controllers
//{
//    [Area("master")]
//    public class EmployeeController : BaseController
//    {
//        private readonly ILogger<EmployeeController> _logger;

//        public EmployeeController(
//            ILogger<EmployeeController> logger

//           )

//        {
//            _logger = logger;
//        }

//        // GET: /master/Employee/Index
//        public async Task<IActionResult> Index()
//        {
//            return View();
//        }

//        // GET: /master/Employee/List
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

//                var apiResponse = await _apiService.GetAsync<List<EmployeeViewModel>>("/master/getemployees", headers);
//                return Json(apiResponse.Data);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while fetching employees.");
//                return Json(null);
//            }
//        }

//        // GET: /master/Employee/GetById
//        [HttpGet]
//        public async Task<JsonResult> GetById(short employeeId, string companyId)
//        {
//            if (employeeId <= 0)
//            {
//                return Json(new { success = false, message = "Invalid Employee ID." });
//            }

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.GetAsync<EmployeeViewModel>($"/master/getemployeebyid/{employeeId}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, data = apiResponse.Data });
//            }
//            else
//            {
//                return Json(new { success = false, message = "Employee not found." });
//            }
//        }

//        // POST: /master/Employee/Save
//        [HttpPost]
//        public async Task<IActionResult> Save([FromBody] SaveEmployeeViewModel model)
//        {
//            if (model == null)
//            {
//                return BadRequest(new { success = false, message = "Data operation failed." });
//            }

//            var employee = model.Employee;
//            var companyId = model.CompanyId;

//            var employeeToSave = new EmployeeViewModel
//            {
//                EmployeeId = employee.EmployeeId,
//                CompanyId = Convert.ToInt16(companyId),
//                EmployeeCode = employee.EmployeeCode ?? string.Empty,
//                EmployeeName = employee.EmployeeName ?? string.Empty,
//                EmployeeOtherName = employee.EmployeeOtherName ?? string.Empty,
//                EmployeePhoto = employee.EmployeePhoto ?? string.Empty,
//                EmployeeSignature = employee.EmployeeSignature ?? string.Empty,
//                DepartmentId = employee.DepartmentId,
//                DepartmentCode = employee.DepartmentCode ?? string.Empty,
//                DepartmentName = employee.DepartmentName ?? string.Empty,
//                EmployeeSex = employee.EmployeeSex ?? string.Empty,
//                MartialStatus = employee.MartialStatus ?? string.Empty,
//                EmployeeDOB = employee.EmployeeDOB,
//                EmployeeJoinDate = employee.EmployeeJoinDate,
//                EmployeeLastDate = employee.EmployeeLastDate,
//                EmployeeOffEmailAdd = employee.EmployeeOffEmailAdd ?? string.Empty,
//                EmployeeOtherEmailAdd = employee.EmployeeOtherEmailAdd ?? string.Empty,
//                Remarks = employee.Remarks?.Trim() ?? string.Empty,
//                IsActive = employee.IsActive,
//                CreateById = employee.CreateById,
//                CreateDate = DateTime.Now,
//                EditById = employee.EditById ?? 0,
//                EditDate = DateTime.Now,
//                CreateBy = employee.CreateBy ?? string.Empty,
//                EditBy = employee.EditBy ?? string.Empty
//            };

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.PostAsync<EmployeeViewModel>("/master/saveemployee", employeeToSave, headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record saved successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Save operation failed." });
//        }

//        // DELETE: /master/Employee/Delete
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

//            var apiResponse = await _apiService.DeleteAsync($"/master/deleteemployee/{id}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record deleted successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Delete operation failed." });
//        }
//    }
//}