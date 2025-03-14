using AEMSWEB.Areas.Account.Models.CB;
using AEMSWEB.Entities.Accounts.CB;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.CB
{
    public interface ICBBankTransferService
    {
        public Task<CBBankTransferViewModelList> GetCBBankTransferListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, short UserId);

        public Task<CBBankTransferViewModel> GetCBBankTransferByIdNoAsync(short CompanyId, long TransferId, string TransferNo, short UserId);

        public Task<SqlResponse> SaveCBBankTransferAsync(short CompanyId, CBBankTransfer cBBankTransfer, short UserId);

        public Task<SqlResponse> DeleteCBBankTransferAsync(short CompanyId, long TransferId, string TransferNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<CBBankTransferViewModel>> GetHistoryCBBankTransferByIdAsync(short CompanyId, long TransferId, string TransferNo, short UserId);
    }
}