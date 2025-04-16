using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Areas.HRM.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}