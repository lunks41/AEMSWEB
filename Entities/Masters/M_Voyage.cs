using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Masters
{
    public class M_Voyage
    {
        public Int16 CompanyId { get; set; }

        [Key]
        public Int16 VoyageId { get; set; }

        public string? VoyageNo { get; set; }
        public string? ReferenceNo { get; set; }

        [ForeignKey(nameof(VesselId))]
        public Int32 VesselId { get; set; }

        [ForeignKey(nameof(BargeId))]
        public Int16 BargeId { get; set; }

        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}