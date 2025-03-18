using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IOrderTypeService
    {
        public Task<OrderTypeViewModelCount> GetOrderTypeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<M_OrderType> GetOrderTypeByIdAsync(short CompanyId, short UserId, short OrderTypeId);

        public Task<SqlResponse> SaveOrderTypeAsync(short CompanyId, short UserId, M_OrderType m_OrderType);

        public Task<SqlResponse> DeleteOrderTypeAsync(short CompanyId, short UserId, M_OrderType m_OrderType);

        public Task<OrderTypeCategoryViewModelCount> GetOrderTypeCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<M_OrderTypeCategory> GetOrderTypeCategoryByIdAsync(short CompanyId, short UserId, int OrderTypeCategoryId);

        public Task<SqlResponse> SaveOrderTypeCategoryAsync(short CompanyId, short UserId, M_OrderTypeCategory m_OrderTypeCategory);

        public Task<SqlResponse> DeleteOrderTypeCategoryAsync(short CompanyId, short UserId, M_OrderTypeCategory m_OrderTypeCategory);
    }
}