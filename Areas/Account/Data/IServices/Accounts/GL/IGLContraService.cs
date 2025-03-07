using AEMSWEB.Areas.Account.Models.GL;
using AEMSWEB.Entities.Accounts.GL;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.GL
{
    public interface IGLContraService
    {
        public Task<GLContraViewModel> GetGLContraListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<GLContraHdViewModel> GetGLContraByIdNoAsync(Int16 CompanyId, Int64 ContraId, string ContraNo, Int16 UserId);

        public Task<SqlResponse> SaveGLContraAsync(Int16 CompanyId, GLContraHd GLContraHd, List<GLContraDt> GLContraDts, Int16 UserId);

        public Task<SqlResponse> DeleteGLContraAsync(Int16 CompanyId, Int64 ContraId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<GLContraHdViewModel>> GetHistoryGLContraByIdAsync(Int16 CompanyId, Int64 ContraId, string ContraNo, Int16 UserId);
    }
}