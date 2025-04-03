using AMESWEB.Data;
using AMESWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AMESWEB.Helpers
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public short ModuleId { get; }
        public short TransactionId { get; }
        public string? PermissionType { get; }

        public PermissionRequirement(short moduleId, short transactionId, string permissionType)
        {
            ModuleId = moduleId;
            TransactionId = transactionId;
            PermissionType = permissionType;
        }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ApplicationDbContext _context;

        public PermissionHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userGroupId = Convert.ToInt16(context.User.FindFirst("UserGroupId")?.Value);

            if (userGroupId == 0)
            {
                context.Fail();
                return;
            }

            var permission = await _context.AdmUserGroupRights
                .FirstOrDefaultAsync(p =>
                    p.UserGroupId == userGroupId &&
                    p.ModuleId == requirement.ModuleId &&
                    p.TransactionId == requirement.TransactionId);

            if (permission != null && HasPermission(permission, requirement.PermissionType))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }

        private bool HasPermission(AdmUserGroupRights permission, string permissionType)
        {
            return permissionType switch
            {
                "Read" => permission.IsRead,
                "Create" => permission.IsCreate,
                "Edit" => permission.IsEdit,
                "Delete" => permission.IsDelete,
                "Export" => permission.IsExport,
                "Print" => permission.IsPrint,
                _ => false
            };
        }
    }

    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        public PermissionAuthorizeAttribute(short moduleId, short transactionId, string permissionType)
        {
            Policy = $"{moduleId}:{transactionId}:{permissionType}";
        }
    }
}