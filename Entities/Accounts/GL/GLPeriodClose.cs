using Microsoft.EntityFrameworkCore;

namespace AEMSWEB.Entities.Accounts.GL
{
    [PrimaryKey(nameof(CompanyId), nameof(FinYear), nameof(FinMonth))]
    public class GLPeriodClose
    {
        public Int16 CompanyId { get; set; }
        public Int32 FinYear { get; set; }
        public Int16 FinMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsArClose { get; set; }
        public Int32 ArCloseById { get; set; }
        public DateTime ArCloseDate { get; set; }
        public bool IsApClose { get; set; }
        public Int32 ApCloseById { get; set; }
        public DateTime ApCloseDate { get; set; }
        public bool IsCbClose { get; set; }
        public Int32 CbCloseById { get; set; }
        public DateTime CbCloseDate { get; set; }
        public bool IsGlClose { get; set; }
        public Int32 GlCloseById { get; set; }
        public DateTime GlCloseDate { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}