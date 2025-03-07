using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Models
{
    [PrimaryKey(nameof(ModuleId), nameof(TransactionId))]
    public class AdmTransaction
    {
        public Int16 TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionName { get; set; }

        [ForeignKey("Module")]
        public byte ModuleId { get; set; }

        [ForeignKey("TransCategory")]
        public Int16 TransCategoryId { get; set; }

        public bool IsNumber { get; set; }
        public Int16 SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }

        public Int16 CreateById { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }

        // Navigation properties
        public AdmModule Module { get; set; }

        public AdmTransactionCategory TransCategory { get; set; }
    }
}