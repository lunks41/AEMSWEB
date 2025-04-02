using System.ComponentModel.DataAnnotations;

namespace AEMSWEB.Areas.HRM.Models
{
    public class Position
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
    }
}
