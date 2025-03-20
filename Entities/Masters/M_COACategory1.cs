using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    public class M_COACategory1
    {
        [Key]
        public Int16 COACategoryId { get; set; }

        public Int16 CompanyId { get; set; }
        public string? COACategoryCode { get; set; }
        public string? COACategoryName { get; set; }
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