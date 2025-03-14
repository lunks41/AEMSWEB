using AEMSWEB.Entities.Admin;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;

namespace AEMSWEB.Areas.Admin.Data
{
    public interface IDocumentService
    {
        public Task<IEnumerable<DocumentViewModel>> GetDocumentByIdAsync(short CompanyId, short ModuleId, short TransactionId, long DocumentId, short UserId);

        public Task<SqlResponse> SaveDocumentAsync(short CompanyId, AdmDocuments admDocuments, short UserId);

        public Task<SqlResponse> DeleteDocumentAsync(short CompanyId, short ModuleId, short TransactionId, long DocumentId, int ItemNo, short UserId);
    }
}