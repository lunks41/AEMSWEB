using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Setting
{
    [PrimaryKey(nameof(CompanyId), nameof(UserId), nameof(ModuleId), nameof(TransactionId), nameof(GrdName))]
    public class S_UserGrdFormat
    {
        public Int16 CompanyId { get; set; }
        public Int16 UserId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public string? GrdName { get; set; }
        public string? GrdKey { get; set; }
        public string? GrdColVisible { get; set; }
        public string? GrdColOrder { get; set; }
        public string? GrdColSize { get; set; }
        public string? GrdSort { get; set; }

        public string? GrdString { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}