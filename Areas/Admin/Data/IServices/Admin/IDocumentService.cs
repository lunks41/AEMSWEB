using AEMSWEB.Entities.Admin;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Admin
{
    public interface IDocumentService
    {
        public Task<IEnumerable<DocumentViewModel>> GetDocumentByIdAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId, Int16 UserId);

        public Task<SqlResponse> SaveDocumentAsync(Int16 CompanyId, AdmDocuments admDocuments, Int16 UserId);

        public Task<SqlResponse> DeleteDocumentAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId, Int32 ItemNo, Int16 UserId);
    }
}