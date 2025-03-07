using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    public class M_PortRegion
    {
        [Key]
        public Int16 PortRegionId { get; set; }

        public Int16 CompanyId { get; set; }
        public string PortRegionCode { get; set; }
        public string PortRegionName { get; set; }

        [ForeignKey("CountryId")]
        public Int16 CountryId { get; set; }

        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}