using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Masters
{
    public class M_AccountSetup
    {
        [Key]
        public Int16 AccSetupId { get; set; }

        public Int16 CompanyId { get; set; }
        public string? AccSetupCode { get; set; }
        public string? AccSetupName { get; set; }
        public Int16 AccSetupCategoryId { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}