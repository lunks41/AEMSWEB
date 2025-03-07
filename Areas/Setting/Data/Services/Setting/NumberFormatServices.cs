using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Enums;
using AEMSWEB.Helper;
using AEMSWEB.IServices;
using AEMSWEB.IServices.Setting;
using AEMSWEB.Models;
using AEMSWEB.Repository;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace AEMSWEB.Services.Setting
{
    public sealed class NumberFormatServices : INumberFormatServices
    {
        private readonly IRepository<S_NumberFormat> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public NumberFormatServices(IRepository<S_NumberFormat> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        // add the number id
        public async Task<ModelNameViewModelCount> GetNumberFormatListAsync(Int16 CompanyId, Int16 UserId)
        {
            ModelNameViewModelCount countViewModel = new ModelNameViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM AdmTransaction AdmTrn INNER JOIN AdmModule AdmMod on AdmMod.ModuleId=AdmTrn.ModuleId where AdmMod.IsActive=1 And AdmTrn.IsActive=1 And AdmTrn.IsNumber=1");

                var result = await _repository.GetQueryAsync<ModelNameViewModel>($"SELECT AdmMod.ModuleId,AdmMod.ModuleName,AdmTrn.TransactionId,AdmTrn.TransactionName FROM AdmTransaction AdmTrn INNER JOIN AdmModule AdmMod on AdmMod.ModuleId=AdmTrn.ModuleId where AdmMod.IsActive=1 And AdmTrn.IsActive=1 And AdmTrn.IsNumber=1  ORDER BY AdmMod.SeqNo,AdmTrn.SeqNo");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result == null ? null : result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocumentNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_NumberFormat",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<NumberSettingViewModel> GetNumberFormatByIdAsync(Int16 CompanyId, Int32 ModuleId, Int32 TransactionId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<NumberSettingViewModel>($"SELECT S_No.NumberId,S_No.CompanyId,S_No.ModuleId,S_No.TransactionId,S_No.Prefix,S_No.PrefixSeq,S_No.PrefixDelimiter,S_No.IncludeYear,S_No.YearSeq,S_No.YearFormat,S_No.YearDelimiter,S_No.IncludeMonth,S_No.MonthSeq,S_No.MonthFormat,S_No.MonthDelimiter,S_No.NoDIgits,S_No.DIgitSeq,S_No.ResetYearly,S_No.CreateById,S_No.CreateDate,S_No.EditById,S_No.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.S_NumberFormat S_No LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = S_No.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = S_No.EditById WHERE S_No.ModuleId={ModuleId} AND S_No.TransactionId={TransactionId} AND S_No.CompanyId={CompanyId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocumentNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_NumberFormat",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<NumberSettingDtViewModel> GetNumberFormatByYearAsync(Int16 CompanyId, Int32 NumberId, Int32 NumYear, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<NumberSettingDtViewModel>($"SELECT NumberId,NumYear,Month1,Month2,Month3,Month4,Month5,Month6,Month7,Month8,Month9,Month10,Month11,Month12,LastNumber FROM dbo.S_NumberFormatDt WHERE NumberId={NumberId} AND NumYear={NumYear}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocumentNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_NumberFormatDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveNumberFormatAsync(Int16 CompanyId, S_NumberFormat s_NumberFormat, Int16 UserId)
        {
            bool IsEdit = s_NumberFormat.NumberId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.S_NumberFormat WHERE ModuleId<>{s_NumberFormat.ModuleId} AND TransactionId<>{s_NumberFormat.TransactionId} AND CompanyId={CompanyId} AND Prefix = '{s_NumberFormat.Prefix}'");

                        if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                            return new SqlResponse { Result = -1, Message = "Invoice Not Exist" };
                    }

                    if (IsEdit)
                    {
                        var entityHead = _context.Update(s_NumberFormat);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>("SELECT ISNULL((SELECT TOP 1 (NumberId + 1) FROM dbo.S_NumberFormat WHERE (NumberId + 1) NOT IN (SELECT NumberId FROM dbo.S_NumberFormat)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            s_NumberFormat.NumberId = Convert.ToInt32(sqlMissingResponse.NextId);

                            s_NumberFormat.EditById = null;
                            s_NumberFormat.EditDate = null;
                            var entity = _context.Add(s_NumberFormat);
                        }
                    }

                    var NumberFormatToSave = _context.SaveChanges();

                    if (NumberFormatToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.DocumentNo,
                            DocumentId = s_NumberFormat.NumberId,
                            DocumentNo = "",
                            TblName = "S_NumberFormat",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "NumberFormat Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 1, Message = "Save Failed" };
                    }

                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.DocumentNo, 0, "", "S_NumberFormat", IsEdit ? E_Mode.Update : E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.DocumentNo, 0, "", "S_NumberFormat", IsEdit ? E_Mode.Update : E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }
    }
}