using AEMSWEB.Areas.Setting.Models;

namespace AEMSWEB.IServices.Setting
{
    public interface IDocSeqNoService
    {
        //public Task<DocSeqNoViewModelCount> GetDocSeqNoListAsync( Int16 CompanyId, Int16 UserId);

        public Task<DocSeqNoViewModel> GetDocSeqNoByTransactionAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId);

        //public Task<SqlResponse> SaveDocSeqNoAsync( Int16 CompanyId, S_DocSeqNo s_DocSeqNo, Int16 UserId);
    }
}