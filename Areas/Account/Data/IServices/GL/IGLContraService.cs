using AEMSWEB.Areas.Account.Models.GL;
using AEMSWEB.Entities.Accounts.GL;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.GL
{
    public interface IGLContraService
    {
        public Task<GLContraViewModel> GetGLContraListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<GLContraHdViewModel> GetGLContraByIdNoAsync(short CompanyId, long ContraId, string ContraNo, short UserId);

        public Task<SqlResponse> SaveGLContraAsync(short CompanyId, GLContraHd GLContraHd, List<GLContraDt> GLContraDts, short UserId);

        public Task<SqlResponse> DeleteGLContraAsync(short CompanyId, long ContraId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<GLContraHdViewModel>> GetHistoryGLContraByIdAsync(short CompanyId, long ContraId, string ContraNo, short UserId);
    }
}