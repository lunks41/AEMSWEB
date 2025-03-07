using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IOrderTypeService
    {
        public Task<OrderTypeViewModelCount> GetOrderTypeListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_OrderType> GetOrderTypeByIdAsync(short CompanyId, short UserId, short OrderTypeId);

        public Task<SqlResponse> SaveOrderTypeAsync(short CompanyId, short UserId, M_OrderType m_OrderType);

        public Task<SqlResponse> DeleteOrderTypeAsync(short CompanyId, short UserId, M_OrderType m_OrderType);
    }
}