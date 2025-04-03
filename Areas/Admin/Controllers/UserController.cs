using AMESWEB.Areas.Admin.Data;
using AMESWEB.Controllers;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Models;
using AMESWEB.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AMESWEB.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _UserService;

        public UserController(ILogger<UserController> logger, IBaseService baseService, IUserService UserService) : base(logger, baseService)
        {
            _logger = logger;
            _UserService = UserService;
        }

        public async Task<IActionResult> Index()
        {
            var companyId = HttpContext.Session.GetString("CurrentCompany");
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }
            var permissions = await HasPermission(companyIdShort, parsedUserId, (short)E_Modules.Admin, (short)E_Admin.User);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.IsExport = permissions?.IsExport ?? false;
            ViewBag.IsPrint = permissions?.IsPrint ?? false;

            return View();
        }

        #region User

        [HttpGet]
        public async Task<JsonResult> UserList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
                {
                    return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
                }

                var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var data = await _UserService.GetUserListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total = total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account groups.");
                return Json(new { Result = -1, Message = "An error occurred", Data = "" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUserById(short selectedUserId, string companyId)
        {
            if (selectedUserId <= 0)
            {
                return Json(new { success = false, message = "Invalid Account Group ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                // Assuming you would have some logic here to use the headers in your service call
                var data = await _UserService.GetUserByIdAsync(selectedUserId, parsedUserId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Account Group not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account group by ID.");
                return Json(new { success = false, message = "An error occurred", data = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser([FromBody] UserViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var userToSave = new AdmUser
            {
                Id = model.UserId,
                UserCode = model.UserCode ?? string.Empty,
                UserName = model.UserName ?? string.Empty,
                NormalizedUserName = model.UserName?.ToUpperInvariant() ?? string.Empty,
                Email = model.Email ?? string.Empty,
                NormalizedEmail = model.Email?.ToUpperInvariant() ?? string.Empty,
                Remarks = model.Remarks?.Trim() ?? string.Empty,
                IsActive = model.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = model.EditById ?? 0,
                EditDate = DateTime.Now,
                PhoneNumber = model.PhoneNumber ?? string.Empty,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false
            };

            try
            {
                var password = model.UserPassword ?? string.Empty; // Assuming the password is passed in the model
                var data = await _UserService.SaveUserAsync(parsedUserId, userToSave, password);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save user." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the user.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(short selectedUserId, string companyId)
        {
            if (selectedUserId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                var userget = await _UserService.GetUserByIdAsync(companyIdShort, parsedUserId);

                var data = await _UserService.DeleteUserAsync(companyIdShort, 1, userget);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account group." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }

        #endregion User

        #region User Group

        [HttpGet]
        public async Task<JsonResult> UserGroupList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
                {
                    return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
                }

                var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var data = await _UserService.GetUserListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total = total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account groups.");
                return Json(new { Result = -1, Message = "An error occurred", Data = "" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUserGroupById(short selectedUserGroupId, string companyId)
        {
            if (selectedUserGroupId <= 0)
            {
                return Json(new { success = false, message = "Invalid Account Group ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                // Assuming you would have some logic here to use the headers in your service call
                var data = await _UserService.GetUserGroupByIdAsync(companyIdShort, parsedUserId, selectedUserGroupId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Account Group not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account group by ID.");
                return Json(new { success = false, message = "An error occurred", data = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserGroup([FromBody] UserGroupViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var userToSave = new AdmUserGroup
            {
                Id = model.UserGroupId,
                UserGroupCode = model.UserGroupCode ?? string.Empty,
                UserGroupName = model.UserGroupName ?? string.Empty,
                Remarks = model.Remarks?.Trim() ?? string.Empty,
                IsActive = model.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = model.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _UserService.SaveUserGroupAsync(parsedUserId, userToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account group." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUserGroup(short selectedUserId, string companyId)
        {
            if (selectedUserId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                var userget = await _UserService.GetUserByIdAsync(companyIdShort, parsedUserId);

                var data = await _UserService.DeleteUserAsync(companyIdShort, 1, userget);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account group." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }

        #endregion User Group

        #region User Rights

        [HttpGet("user/UserRightsList")]
        public async Task<JsonResult> UserRightsList(int pageNumber, int pageSize, string searchString, string companyId, int selecteduserId)
        {
            if (selecteduserId == 0)
            {
                return Json(new { data = "", total = 0 });
            }
            try
            {
                if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
                {
                    return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
                }

                var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var data = await _UserService.GetUserRightsByIdAsync(companyIdShort, parsedUserId, selecteduserId);

                var paginatedData = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account groups.");
                return Json(new { Result = -1, Message = "An error occurred", Data = "" });
            }
        }

        [HttpPost("user/SaveUserRights")]
        public async Task<IActionResult> SaveUserRights([FromBody] SaveUserRightsModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var userCurrenctId = model.SelectedUserId;
            var usergroup = model.UserRights;

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var userToSave = usergroup
                    .Where(item => item.IsAccess)
                    .Select(item => new AdmUserRights
                    {
                        CompanyId = item.CompanyId,
                        UserId = userCurrenctId, // Use the user ID from the model
                        CreateById = parsedUserId,
                    })
                    .ToList();

            try
            {
                var data = await _UserService.SaveUserRightsAsync(1, parsedUserId, userToSave, userCurrenctId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account group." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }

        #endregion User Rights

        #region User Group Rights

        [HttpGet("user/UserGroupRightsList")]
        public async Task<JsonResult> UserGroupRightsList(int pageNumber, int pageSize, string searchString, string companyId, int SelectedGroupId)
        {
            try
            {
                if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
                {
                    return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
                }

                var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var data = await _UserService.GetUserGroupRightsByIdAsync(companyIdShort, parsedUserId, SelectedGroupId);

                var paginatedData = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account groups.");
                return Json(new { Result = -1, Message = "An error occurred", Data = "" });
            }
        }

        [HttpPost("user/SaveUserGroupRights")]
        public async Task<IActionResult> SaveUserGroupRights([FromBody] SaveUserGroupRightsModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var userCurrenctId = model.SelectedUserId;
            var userModuleId = model.SelectedModuleId;
            var usergrouprights = model.UserGroupRights;

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var userToSave = usergrouprights
                    .Where(item => item.IsCreate)
                    .Select(item => new AdmUserRights
                    {
                        UserId = userCurrenctId, // Use the user ID from the model
                        CreateById = parsedUserId,
                    })
                    .ToList();

            try
            {
                var data = await _UserService.SaveUserRightsAsync(1, parsedUserId, userToSave, userCurrenctId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account group." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }

        #endregion User Group Rights
    }
}