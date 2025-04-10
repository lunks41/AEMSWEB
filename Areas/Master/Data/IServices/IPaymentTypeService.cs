using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IPaymentTypeService
    {
        public Task<PaymentTypeViewModelCount> GetPaymentTypeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<PaymentTypeViewModel> GetPaymentTypeByIdAsync(short CompanyId, short UserId, short PaymentTypeId);

        public Task<SqlResponce> SavePaymentTypeAsync(short CompanyId, short UserId, M_PaymentType m_PaymentType);

        public Task<SqlResponce> DeletePaymentTypeAsync(short CompanyId, short UserId, short PaymentTypeId);
    }
}