using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IOrderTypeService
    {
        public Task<OrderTypeViewModelCount> GetOrderTypeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<OrderTypeViewModel> GetOrderTypeByIdAsync(short CompanyId, short UserId, short OrderTypeId);

        public Task<SqlResponse> SaveOrderTypeAsync(short CompanyId, short UserId, M_OrderType m_OrderType);

        public Task<SqlResponse> DeleteOrderTypeAsync(short CompanyId, short UserId, short OrderTypeId);

        public Task<OrderTypeCategoryViewModelCount> GetOrderTypeCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<OrderTypeCategoryViewModel> GetOrderTypeCategoryByIdAsync(short CompanyId, short UserId, short OrderTypeCategoryId);

        public Task<SqlResponse> SaveOrderTypeCategoryAsync(short CompanyId, short UserId, M_OrderTypeCategory m_OrderTypeCategory);

        public Task<SqlResponse> DeleteOrderTypeCategoryAsync(short CompanyId, short UserId, short OrderTypeCategoryId);
    }
}