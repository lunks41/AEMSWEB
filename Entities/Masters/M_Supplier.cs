using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Masters
{
    public class M_Supplier
    {
        [Key]
        public Int32 SupplierId { get; set; }

        public Int16 CompanyId { get; set; }
        public string? SupplierCode { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierOtherName { get; set; }
        public string? SupplierShortName { get; set; }
        public string? SupplierRegNo { get; set; }
        public Int16 CurrencyId { get; set; }
        public Int16 CreditTermId { get; set; }
        public Int32 ParentSupplierId { get; set; }
        public Int16 AccSetupId { get; set; }
        public Int32 CustomerId { get; set; }
        public Int16 BankId { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsVendor { get; set; }
        public bool IsTrader { get; set; }
        public bool IsCustomer { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}