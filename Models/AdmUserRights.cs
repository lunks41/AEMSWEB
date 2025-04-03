namespace AMESWEB.Models
{
    public class AdmUserRights
    {
        public Int16 CompanyId { get; set; }
        public Int16 UserId { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}