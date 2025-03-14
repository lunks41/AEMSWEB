using AEMSWEB.Areas.Account.Models.CB;
using AEMSWEB.Entities.Accounts.CB;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.CB
{
    public interface ICBBankReconService
    {
        public Task<CBBankReconViewModel> GetCBBankReconListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<CBBankReconHdViewModel> GetCBBankReconByIdNoAsync(short CompanyId, long ReconId, string ReconNo, short UserId);

        public Task<SqlResponse> SaveCBBankReconAsync(short CompanyId, CBBankReconHd CBBankReconHd, List<CBBankReconDt> CBBankReconDt, short UserId);

        public Task<SqlResponse> DeleteCBBankReconAsync(short CompanyId, long ReconId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<CBBankReconHdViewModel>> GetHistoryCBBankReconByIdAsync(short CompanyId, long ReconId, string ReconNo, short UserId);
    }
}