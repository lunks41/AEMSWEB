using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Accounts.GL
{
    [PrimaryKey(nameof(DocumentId), nameof(ItemNo))]
    public class GLOpeningBalance
    {
        public Int16 CompanyId { get; set; }
        public Int32 DocumentId { get; set; }
        public Int32 ItemNo { get; set; }
        public Int32 GLId { get; set; }
        public string DocumentNo { get; set; }
        public DateTime AccountDate { get; set; }
        public Int32 CustomerId { get; set; }
        public Int32 SupplierId { get; set; }
        public Int16 CurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        public bool IsDebit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        public Int32 DepartmentId { get; set; }
        public Int32 EmployeeId { get; set; }
        public Int32 ProductId { get; set; }
        public Int32 PortId { get; set; }
        public Int32 VesselId { get; set; }
        public Int32 BargeId { get; set; }
        public Int32 VoyageId { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public byte EditVersion { get; set; }
    }
}