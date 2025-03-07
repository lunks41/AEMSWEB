using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Masters
{
    public interface IErrorLogService
    {
        public Task<IEnumerable<ErrorLogViewModel>> GetErrorLogListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);
    }
}