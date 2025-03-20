using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Accounts.GL
{
    [PrimaryKey(nameof(ContraId))]
    public class GLContraHd
    {
        [ForeignKey(nameof(CompanyId))]
        public Int16 CompanyId { get; set; }

        [Key]
        public Int64 ContraId { get; set; }

        public string? ContraNo { get; set; }
        public string? ReferenceNo { get; set; }

        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Int32 CustomerId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Int32 SupplierId { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public Int16 CurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExhGainLoss { get; set; }

        public string? Remarks { get; set; }
        public string? ModuleFrom { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public Int16? CancelById { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancelRemarks { get; set; }
        public byte EditVersion { get; set; }
    }
}