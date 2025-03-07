using AEMSWEB.Areas.Account.Models.CB;
using AEMSWEB.Entities.Accounts.CB;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.CB
{
    public interface ICBBankReconService
    {
        public Task<CBBankReconViewModel> GetCBBankReconListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<CBBankReconHdViewModel> GetCBBankReconByIdNoAsync(Int16 CompanyId, Int64 ReconId, string ReconNo, Int16 UserId);

        public Task<SqlResponse> SaveCBBankReconAsync(Int16 CompanyId, CBBankReconHd CBBankReconHd, List<CBBankReconDt> CBBankReconDt, Int16 UserId);

        public Task<SqlResponse> DeleteCBBankReconAsync(Int16 CompanyId, Int64 ReconId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<CBBankReconHdViewModel>> GetHistoryCBBankReconByIdAsync(Int16 CompanyId, Int64 ReconId, string ReconNo, Int16 UserId);
    }
}