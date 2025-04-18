﻿using AMESWEB.Areas.Account.Models.AR;
using AMESWEB.Entities.Accounts.AR;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AR
{
    public interface IARRefundService
    {
        public Task<ARRefundViewModelCount> GetARRefundListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<ARRefundViewModel> GetARRefundByIdAsync(short CompanyId, long RefundId, string RefundNo, short UserId);

        public Task<SqlResponce> SaveARRefundAsync(short CompanyId, ArRefundHd ARRefundHd, List<ArRefundDt> ARRefundDt, short UserId);

        public Task<SqlResponce> DeleteARRefundAsync(short CompanyId, long RefundId, string RefundNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<ARRefundViewModel>> GetHistoryARRefundByIdAsync(short CompanyId, long RefundId, string RefundNo, short UserId);
    }
}