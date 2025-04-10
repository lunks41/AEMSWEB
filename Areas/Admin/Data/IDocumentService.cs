using AMESWEB.Entities.Admin;
using AMESWEB.Models;
using AMESWEB.Models.Admin;

namespace AMESWEB.Areas.Admin.Data
{
    public interface IDocumentService
    {
        public Task<IEnumerable<DocumentViewModel>> GetDocumentByIdAsync(short CompanyId, short ModuleId, short TransactionId, long DocumentId, short UserId);

        public Task<SqlResponce> SaveDocumentAsync(short CompanyId, AdmDocuments admDocuments, short UserId);

        public Task<SqlResponce> DeleteDocumentAsync(short CompanyId, short ModuleId, short TransactionId, long DocumentId, int ItemNo, short UserId);
    }
}