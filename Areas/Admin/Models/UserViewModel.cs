namespace AEMSWEB.Models.Admin
{
    public class UserViewModel
    {
        public Int16 UserId { get; set; }
        public string? UserCode { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
        public string? UserEmail { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public Int16 UserGroupId { get; set; }
        public string? UserGroupCode { get; set; }
        public string? UserGroupName { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
        public string? Email { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? NormalizedEmail { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}