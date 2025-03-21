using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Controllers;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class EmployeeController : BaseController
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger,
            IBaseService baseService,
            IEmployeeService employeeService)
            : base(logger, baseService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        #region Employee CRUD

        [Authorize]
        public async Task<IActionResult> Index(int? companyId)
        {
            if (!companyId.HasValue || companyId <= 0)
            {
                _logger.LogWarning("Invalid company ID: {CompanyId}", companyId);
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var parsedUserId = GetParsedUserId();
            if (!parsedUserId.HasValue)
            {
                _logger.LogWarning("User not logged in or invalid user ID.");
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var permissions = await HasPermission((short)companyId, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Employee);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _employeeService.GetEmployeeListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employee list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(short employeeId, string companyId)
        {
            if (employeeId <= 0)
                return Json(new { success = false, message = "Invalid Employee ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _employeeService.GetEmployeeByIdAsync(companyIdShort, parsedUserId.Value, employeeId);
                return data == null
                    ? Json(new { success = false, message = "Employee not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employee by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveEmployeeViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var employeeToSave = new M_Employee
                {
                    EmployeeId = model.employee.EmployeeId,
                    CompanyId = companyIdShort,
                    EmployeeCode = model.employee.EmployeeCode ?? string.Empty,
                    EmployeeName = model.employee.EmployeeName ?? string.Empty,
                    EmployeeOtherName = model.employee.EmployeeOtherName ?? string.Empty,
                    EmployeePhoto = model.employee.EmployeePhoto ?? string.Empty,
                    EmployeeSignature = model.employee.EmployeeSignature ?? string.Empty,
                    DepartmentId = model.employee.DepartmentId,
                    EmployeeSex = model.employee.EmployeeSex ?? string.Empty,
                    MartialStatus = model.employee.MartialStatus ?? string.Empty,
                    EmployeeDOB = model.employee.EmployeeDOB,
                    EmployeeJoinDate = model.employee.EmployeeJoinDate,
                    EmployeeLastDate = model.employee.EmployeeLastDate,
                    EmployeeOffEmailAdd = model.employee.EmployeeOffEmailAdd ?? string.Empty,
                    EmployeeOtherEmailAdd = model.employee.EmployeeOtherEmailAdd ?? string.Empty,
                    Remarks = model.employee.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.employee.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.UtcNow
                };

                var result = await _employeeService.SaveEmployeeAsync(companyIdShort, parsedUserId.Value, employeeToSave);
                return Json(new { success = true, message = "Employee saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving employee");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short employeeId, string companyId)
        {
            if (employeeId <= 0)
                return Json(new { success = false, message = "Invalid Employee ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Employee);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(companyIdShort, parsedUserId.Value, employeeId);
                if (employee == null)
                    return Json(new { success = false, message = "Employee not found" });

                await _employeeService.DeleteEmployeeAsync(companyIdShort, parsedUserId.Value, employee);
                return Json(new { success = true, message = "Employee deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Employee CRUD
    }
}