using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IPaymentTypeService
    {
        public Task<PaymentTypeViewModelCount> GetPaymentTypeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<PaymentTypeViewModel> GetPaymentTypeByIdAsync(short CompanyId, short UserId, short PaymentTypeId);

        public Task<SqlResponse> SavePaymentTypeAsync(short CompanyId, short UserId, M_PaymentType m_PaymentType);

        public Task<SqlResponse> DeletePaymentTypeAsync(short CompanyId, short UserId, short PaymentTypeId);
    }
}