using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Models.Masters
{
    public class UomDtViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 UomId { get; set; }
        public string UomCode { get; set; }
        public string UomName { get; set; }
        public Int16 PackUomId { get; set; }
        public string PackUomCode { get; set; }
        public string PackUomName { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal UomFactor { get; set; }

        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}