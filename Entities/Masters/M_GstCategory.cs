using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Masters
{
    public class M_GstCategory
    {
        [Key]
        public Int16 GstCategoryId { get; set; }

        public Int16 CompanyId { get; set; }
        public string? GstCategoryCode { get; set; }
        public string? GstCategoryName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}