using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    [PrimaryKey(nameof(SupplierId), nameof(SupplierBankId))]
    public class M_SupplierBank
    {
        public Int16 SupplierBankId { get; set; }
        public Int32 SupplierId { get; set; }
        public Int16 BankId { get; set; }
        public string? BranchName { get; set; }
        public string? AccountNo { get; set; }
        public string? SwiftCode { get; set; }
        public string? OtherCode { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? PinCode { get; set; }
        public Int16 CountryId { get; set; }
        public string? Remarks1 { get; set; }
        public string? Remarks2 { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}