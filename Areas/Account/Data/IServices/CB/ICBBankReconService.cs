using AMESWEB.Areas.Account.Models.CB;
using AMESWEB.Entities.Accounts.CB;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.CB
{
    public interface ICBBankReconService
    {
        public Task<CBBankReconViewModel> GetCBBankReconListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<CBBankReconHdViewModel> GetCBBankReconByIdNoAsync(short CompanyId, long ReconId, string ReconNo, short UserId);

        public Task<SqlResponce> SaveCBBankReconAsync(short CompanyId, CBBankReconHd CBBankReconHd, List<CBBankReconDt> CBBankReconDt, short UserId);

        public Task<SqlResponce> DeleteCBBankReconAsync(short CompanyId, long ReconId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<CBBankReconHdViewModel>> GetHistoryCBBankReconByIdAsync(short CompanyId, long ReconId, string ReconNo, short UserId);
    }
}