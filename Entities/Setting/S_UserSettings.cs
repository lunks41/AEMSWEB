using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Setting
{
    [PrimaryKey(nameof(CompanyId), nameof(UserId))]
    public class S_UserSettings
    {
        public Int16 CompanyId { get; set; }
        public Int16 UserId { get; set; }
        public Int16 Trn_Grd_TotRec { get; set; }
        public Int16 M_Grd_TotRec { get; set; }

        [ForeignKey("GLId")]
        public Int32 Ar_IN_GLId { get; set; }

        [ForeignKey("GLId")]
        public Int32 Ar_CN_GLId { get; set; }

        [ForeignKey("GLId")]
        public Int32 Ar_DN_GLId { get; set; }

        [ForeignKey("GLId")]
        public Int32 Ap_IN_GLId { get; set; }

        [ForeignKey("GLId")]
        public Int32 Ap_CN_GLId { get; set; }

        [ForeignKey("GLId")]
        public Int32 Ap_DN_GLId { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}