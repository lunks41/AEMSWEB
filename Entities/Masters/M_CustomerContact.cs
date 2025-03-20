using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    [PrimaryKey(nameof(CustomerId), nameof(ContactId))]
    public class M_CustomerContact
    {
        public Int16 ContactId { get; set; }

        public Int32 CustomerId { get; set; }

        public string? ContactName { get; set; }
        public string? OtherName { get; set; }
        public string? MobileNo { get; set; }
        public string? OffNo { get; set; }
        public string? FaxNo { get; set; }
        public string? EmailAdd { get; set; }
        public string? MessId { get; set; }
        public string? ContactMessType { get; set; }
        public bool IsDefault { get; set; }
        public bool IsFinance { get; set; }
        public bool IsSales { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}