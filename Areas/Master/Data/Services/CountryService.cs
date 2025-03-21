using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.Helpers;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;
using AEMSWEB.Repository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Master.Data.Services
{
    public sealed class CountryService : ICountryService
    {
        private readonly IRepository<M_Country> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public CountryService(IRepository<M_Country> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<CountryViewModelCount> GetCountryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            CountryViewModelCount countViewModel = new CountryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Country M_Cou WHERE (M_Cou.CountryName LIKE '%{searchString}%' OR M_Cou.CountryCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.CountryId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Country}))");

                var result = await _repository.GetQueryAsync<CountryViewModel>($"SELECT M_Cou.CountryId,M_Cou.CountryCode,M_Cou.CountryName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Country M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.CountryName LIKE '%{searchString}%' OR M_Cou.CountryCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.CountryId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Country})) ORDER BY M_Cou.CountryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Country,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Country",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CountryViewModel> GetCountryByIdAsync(short CompanyId, short UserId, short CountryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CountryViewModel>($"SELECT CountryId,CountryCode,CountryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Country WHERE CountryId={CountryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Country}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Country,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Country",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCountryAsync(short CompanyId, short UserId, M_Country m_Country)
        {

            bool IsEdit = m_Country.CountryId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Country WHERE CountryId<>@CountryId AND CountryCode=@CountryCode",
                        new { m_Country.CountryId, m_Country.CountryCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Country Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Country WHERE CountryId<>@CountryId AND CountryName=@CountryName",
                        new { m_Country.CountryId, m_Country.CountryName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Country Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Country WHERE CountryId=@CountryId",
                            new { m_Country.CountryId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Country);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Country Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (CountryId + 1) FROM dbo.M_Country WHERE (CountryId + 1) NOT IN (SELECT CountryId FROM dbo.M_Country)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Country.CountryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Country);
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Country,
                            DocumentId = m_Country.CountryId,
                            DocumentNo = m_Country.CountryCode,
                            TblName = "M_Country",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Country Save Successfully",
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

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory1,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory1",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponse
                {
                    Result = -1,
                    Message = errorMessage
                };
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Country,
                    DocumentId = m_Country.CountryId,
                    DocumentNo = m_Country.CountryCode,
                    TblName = "AdmUser",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponse> DeleteCountryAsync(short CompanyId, short UserId, short countryId)
        {
            string countryNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    countryNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT CountryCode FROM dbo.M_Country WHERE CountryId={countryId}");

                    if (countryId > 0)
                    {
                        var accountGroupToRemove = _context.M_Country
                            .Where(x => x.CountryId == countryId)
                            .ExecuteDelete();


                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Country,
                                DocumentId = countryId,
                                DocumentNo = countryNo,
                                TblName = "M_Country",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Country Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "CountryId Should be zero" };
                    }
                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Country,
                    DocumentId = countryId,
                    DocumentNo = "",
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponse
                {
                    Result = -1,
                    Message = errorMessage
                };
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Country,
                    DocumentId = countryId,
                    DocumentNo = "",
                    TblName = "M_Country",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<DataSet> GetTrainingByIdsAsync(int Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Type", "GET_BY_TRAINING_ID", DbType.String);
                parameters.Add("Id", Id, DbType.Int32);
                return await _repository.GetExecuteDataSetStoredProcedure("USP_LMS_Training", parameters);
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Exception: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}