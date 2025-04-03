namespace AMESWEB.Models
{
    public class Permission
    {
        public Guid CompanyId { get; set; }
        public string? UserId { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}