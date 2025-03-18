using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IDocumentTypeService
    {
        public Task<DocumentTypeViewModelCount> GetDocumentTypeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<DocumentTypeViewModel> GetDocumentTypeByIdAsync(short CompanyId, short UserId, short DocTypeId);

        public Task<SqlResponse> SaveDocumentTypeAsync(short CompanyId, short UserId, M_DocumentType m_DocumentType);

        public Task<SqlResponse> DeleteDocumentTypeAsync(short CompanyId, short UserId, DocumentTypeViewModel documentTypeViewModel);
    }
}