using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Masters
{
    public class M_Bank
    {
        [Key]
        public Int16 BankId { get; set; }

        public Int16 CompanyId { get; set; }
        public string? BankCode { get; set; }
        public string? BankName { get; set; }
        public Int16 CurrencyId { get; set; }
        public string? AccountNo { get; set; }
        public string? SwiftCode { get; set; }
        public string? Remarks1 { get; set; }
        public string? Remarks2 { get; set; }
        public string? Remarks3 { get; set; }
        public Int16 GLId { get; set; }
        public bool IsActive { get; set; }
        public bool IsOwnBank { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}