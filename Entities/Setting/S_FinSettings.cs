using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Setting
{
    public class S_FinSettings
    {
        [Key]
        public Int16 CompanyId { get; set; }

        [ForeignKey("CurrencyId")]
        public Int16 Base_CurrencyId { get; set; }

        public Int16 Local_CurrencyId { get; set; }

        [ForeignKey("GLId")]
        public Int32 ExhGainLoss_GlId { get; set; }

        [ForeignKey("GLId")]
        public Int32 BankCharge_GlId { get; set; }

        [ForeignKey("GLId")]
        public Int32 ProfitLoss_GlId { get; set; }

        [ForeignKey("GLId")]
        public Int32 RetEarning_GlId { get; set; }

        [ForeignKey("GLId")]
        public Int32 SaleGst_GlId { get; set; }

        [ForeignKey("GLId")]
        public Int32 PurGst_GlId { get; set; }

        [ForeignKey("GLId")]
        public Int32 SaleDef_GlId { get; set; }

        [ForeignKey("GLId")]
        public Int32 PurDef_GlId { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}