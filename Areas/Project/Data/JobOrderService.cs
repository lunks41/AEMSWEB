using AEMSWEB.Areas.Project.Data.IServices;
using AEMSWEB.Areas.Project.Models;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Project;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Repository;

namespace AEMSWEB.Areas.Project.Data.Services
{
    public sealed class JoborderService : IJobOrderService
    {
        private readonly IRepository<Ser_JobOrderHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public JoborderService(IRepository<Ser_JobOrderHd> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<JobOrderViewModelCount> GetJobOrderListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            JobOrderViewModelCount countViewModel = new JobOrderViewModelCount();
            try
            {
                // Count query for total records
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                    $"SELECT COUNT(*) AS CountId FROM dbo.Ser_JobOrderHd Hd " +
                    $"LEFT JOIN dbo.M_Currency Cur ON Cur.CurrencyId = Hd.CurrencyId " +
                    $"WHERE (Cur.CurrencyName LIKE '%{searchString}%' " +
                    $"OR Cur.CurrencyCode LIKE '%{searchString}%' " +
                    $"OR Hd.JobOrderNo LIKE '%{searchString}%' " +
                    $"OR Hd.Remarks LIKE '%{searchString}%') " +
                    $"AND Hd.JobOrderId <> 0 AND Hd.CompanyId = {CompanyId}");

                // Query to fetch paginated data
                var result = await _repository.GetQueryAsync<JobOrderHdViewModel>(
                    $"SELECT Hd.JobOrderId, Hd.CompanyId, Hd.JobOrderNo, Hd.JobOrderDate, Hd.CustomerId, Cur.CurrencyId,Cut.CustomerCode,Cut.CustomerName,Hd.IMONo, " +
                    $"Cur.CurrencyName, Cur.CurrencyCode, Hd.TotalAmt, Hd.TotalLocalAmt, Hd.Remarks, Hd.IsActive, Hd.IsClose, " +
                    $"Usr.UserName AS CreateBy, Usr1.UserName AS EditBy " +
                    $"FROM dbo.Ser_JobOrderHd Hd " +
                    $"INNER JOIN dbo.M_Customer Cut ON Cut.CustomerId = Hd.CustomerId " +
                    $"INNER JOIN dbo.M_Currency Cur ON Cur.CurrencyId = Hd.CurrencyId " +
                    $"LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Hd.CreateById " +
                    $"LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Hd.EditById " +
                    $"WHERE (Cur.CurrencyName LIKE '%{searchString}%' " +
                    $"OR Cur.CurrencyCode LIKE '%{searchString}%' " +
                    $"OR Hd.JobOrderNo LIKE '%{searchString}%' " +
                    $"OR Hd.Remarks LIKE '%{searchString}%') " +
                    $"AND Hd.JobOrderId <> 0 AND Hd.CompanyId = {CompanyId} " +
                    $"ORDER BY Hd.JobOrderNo " +
                    $"OFFSET {pageSize} * ({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<JobOrderHdViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Job,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "Ser_JobOrderHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<JobOrderHdViewModel> GetJobOrderByIdAsync(short CompanyId, short UserId, int JobOrderId, string JobOrderHdCode, string JobOrderHdName)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<JobOrderHdViewModel>($"SELECT\r\nM_Ban.JobOrderId,M_Ban.CompanyId,M_Ban.JobOrderHdCode,M_Ban.JobOrderHdName,M_Ban.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Ban.AccountNo,M_Ban.SwiftCode,M_Ban.Remarks1,M_Ban.Remarks2,M_Ban.IsActive,M_Ban.IsOwnJobOrderHd,M_Ban.GLId,M_Chr.GLName,M_Chr.GLCode,M_Ban.CreateById,M_Ban.CreateDate,M_Ban.EditById,M_Ban.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_JobOrderHd M_Ban INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Ban.GLId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Ban.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Ban.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Ban.EditById WHERE (M_Ban.JobOrderId={JobOrderId} OR {JobOrderId}=0) AND (M_Ban.JobOrderHdCode='{JobOrderHdCode}' OR '{JobOrderHdCode}'='{string.Empty}') AND (M_Ban.JobOrderHdName='{JobOrderHdName}' OR '{JobOrderHdName}'='{string.Empty}') AND M_Ban.CompanyId={CompanyId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Job,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_JobOrderHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

    }
}