using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Accounts.GL
{
    [PrimaryKey(nameof(ContraId), nameof(ItemNo))]
    public class GLContraDt
    {
        public Int64 ContraId { get; set; }
        public string ContraNo { get; set; }
        public Int16 ItemNo { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public Int64 DocumentId { get; set; }
        public string DocumentNo { get; set; }
        public string DocReferenceNo { get; set; }
        public Int16 DocCurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal DocExhRate { get; set; }

        public DateTime DocAccountDate { get; set; }
        public DateTime DocDueDate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocTotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocBalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocBalLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AllocAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AllocLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocAllocAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocAllocLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CentDiff { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExhGainLoss { get; set; }

        public byte EditVersion { get; set; }
    }
}