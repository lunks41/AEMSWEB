using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    public class M_SubCategory
    {
        [Key]
        public Int16 SubCategoryId { get; set; }

        public Int16 CompanyId { get; set; }
        public string? SubCategoryCode { get; set; }
        public string? SubCategoryName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}