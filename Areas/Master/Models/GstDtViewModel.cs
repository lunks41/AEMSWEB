using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Models.Masters
{
    public class GstDtViewModel
    {
        public Int16 GstId { get; set; }
        public string GstCode { get; set; }
        public string GstName { get; set; }
        public Int16 CompanyId { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal GstPercentahge { get; set; }

        public DateTime ValidFrom { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}