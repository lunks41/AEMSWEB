using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IOrderTypeCategoryService
    {
        public Task<OrderTypeCategoryViewModelCount> GetOrderTypeCategoryListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_OrderTypeCategory> GetOrderTypeCategoryByIdAsync(short CompanyId, short UserId, int OrderTypeCategoryId);

        public Task<SqlResponse> SaveOrderTypeCategoryAsync(short CompanyId, short UserId, M_OrderTypeCategory m_OrderTypeCategory);

        public Task<SqlResponse> DeleteOrderTypeCategoryAsync(short CompanyId, short UserId, M_OrderTypeCategory m_OrderTypeCategory);
    }
}