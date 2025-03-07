using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    public class M_Customer
    {
        [Key]
        public Int32 CustomerId { get; set; }

        public Int16 CompanyId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerOtherName { get; set; }
        public string CustomerShortName { get; set; }
        public string CustomerRegNo { get; set; }
        public Int16 CurrencyId { get; set; }
        public Int16 CreditTermId { get; set; }
        public Int32 ParentCustomerId { get; set; }
        public Int16 AccSetupId { get; set; }
        public Int32 SupplierId { get; set; }
        public Int16 BankId { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsVendor { get; set; }
        public bool IsTrader { get; set; }
        public bool IsSupplier { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}