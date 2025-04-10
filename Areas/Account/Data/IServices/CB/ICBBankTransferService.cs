using AMESWEB.Areas.Account.Models.CB;
using AMESWEB.Entities.Accounts.CB;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.CB
{
    public interface ICBBankTransferService
    {
        public Task<CBBankTransferViewModelList> GetCBBankTransferListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, short UserId);

        public Task<CBBankTransferViewModel> GetCBBankTransferByIdNoAsync(short CompanyId, long TransferId, string TransferNo, short UserId);

        public Task<SqlResponce> SaveCBBankTransferAsync(short CompanyId, CBBankTransfer cBBankTransfer, short UserId);

        public Task<SqlResponce> DeleteCBBankTransferAsync(short CompanyId, long TransferId, string TransferNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<CBBankTransferViewModel>> GetHistoryCBBankTransferByIdAsync(short CompanyId, long TransferId, string TransferNo, short UserId);
    }
}