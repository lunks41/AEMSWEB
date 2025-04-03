using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Admin
{
    [PrimaryKey(nameof(CompanyId), nameof(ModuleId), nameof(TransactionId), nameof(DocumentId), nameof(ItemNo))]
    public class AdmDocuments
    {
        public Int16 CompanyId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public Int64 DocumentId { get; set; }
        public string? DocumentNo { get; set; }
        public Int32 ItemNo { get; set; }
        public Int16 DocTypeId { get; set; }
        public string? DocPath { get; set; }
        public string? Remarks { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int32? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}