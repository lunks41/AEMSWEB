using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Masters
{
    public class M_Port
    {
        [Key]
        public Int16 PortId { get; set; }

        public Int16 CompanyId { get; set; }

        [ForeignKey("PortRegionId")]
        public Int16 PortRegionId { get; set; }

        public string? PortCode { get; set; }
        public string? PortName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}