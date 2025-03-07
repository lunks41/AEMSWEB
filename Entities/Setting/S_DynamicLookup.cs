using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Setting
{
    public class S_DynamicLookup
    {
        [Key]
        public Int16 CompanyId { get; set; }

        public bool IsBarge { get; set; }
        public bool IsVessel { get; set; }
        public bool IsVoyage { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsProduct { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}