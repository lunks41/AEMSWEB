using System.ComponentModel.DataAnnotations;

namespace AEMSWEB.Models
{
    public class AdmTransactionCategory
    {
        [Key]
        public Int16 TransCategoryId { get; set; }

        public string TransCategoryCode { get; set; }
        public string TransCategoryName { get; set; }
        public Int16 SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }

        // Navigation property
        public ICollection<AdmTransaction> Transactions { get; set; }
    }
}