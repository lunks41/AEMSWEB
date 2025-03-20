using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    public class M_ChartOfAccount
    {
        [Key]
        public Int16 GLId { get; set; }

        public Int16 CompanyId { get; set; }
        public string? GLCode { get; set; }
        public string? GLName { get; set; }
        public Int16 AccTypeId { get; set; }
        public Int16 AccGroupId { get; set; }

        [ForeignKey("COACategoryId1")]
        public Int16 COACategoryId1 { get; set; }

        [ForeignKey("COACategoryId2")]
        public Int16 COACategoryId2 { get; set; }

        [ForeignKey("COACategoryId3")]
        public Int16 COACategoryId3 { get; set; }

        public bool IsSysControl { get; set; }
        public Int16 SeqNo { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}