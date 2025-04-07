using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Setting
{
    public class S_TaskSettings
    {
        [Key]
        public Int16 CompanyId { get; set; }

        [Key]
        public Int16 TaskId { get; set; }

        public Int16 GlId { get; set; }

        public Int32 ChargeId { get; set; }

        public Int32 UomId { get; set; }

        public Int32 TypeId { get; set; }

        public Int32 VisaTypeId { get; set; }

        public Int32 StatusId { get; set; }

        public Int32 LocationId { get; set; }

        public Int32 ServiceTypeId { get; set; }

        public Int32 PassTypeId { get; set; }
        public Int32 CaluateId { get; set; }
        public Int32 BerthTypeId { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}