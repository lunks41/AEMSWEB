using System.ComponentModel.DataAnnotations;

namespace AEMSWEB.Models
{
    public class Base<T>
    {
        [Key]
        public T Id { get; set; }

        public DateTime? EntryDate { get; set; }

        //public Int32 EntryBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        //public Int32 UpdateBy { get; set; }
        //public Int32 CompanyId { get; set; }
    }
}