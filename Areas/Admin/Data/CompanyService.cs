using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Enums;
using AMESWEB.Helpers;
using AMESWEB.Models;
using AMESWEB.Models.Admin;
using AMESWEB.Repository;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace AMESWEB.Areas.Admin.Data
{
    public sealed class CompanyService : ICompanyService
    {
        private readonly IRepository<AdmCompany> _repository;
        private ApplicationDbContext _context;

        public CompanyService(IRepository<AdmCompany> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<CompanyViewModel>> GetUserCompanyListAsync(short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<CompanyViewModel>($"SELECT CompanyId,CompanyName FROM AdmCompany WHERE IsActive=1 AND CompanyId IN (SELECT CompanyId FROM AdmUserRights WHERE UserId={UserId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetUserLoginCompany",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CompanyViewModelCount> GetCompanyListAsync(int pageSize, int pageNumber, string searchString)
        {
            CompanyViewModelCount countViewModel = new CompanyViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.AdmCompany M_ACC  LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACC.EditById WHERE (M_ACC.CompanyName LIKE '%{searchString}%' OR M_ACC.CompanyCode LIKE '%{searchString}%' OR M_ACC.Remarks LIKE '%{searchString}%')");

                var result = await _repository.GetQueryAsync<CompanyViewModel>($"SELECT M_ACC.CompanyId,M_ACC.CompanyCode,M_ACC.CompanyName,,M_ACC.CompanyId,M_ACC.Remarks,M_ACC.IsActive,M_ACC.CreateById,M_ACC.CreateDate,M_ACC.EditById,M_ACC.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.AdmCompany M_ACC  LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACC.EditById WHERE (M_ACC.CompanyName LIKE '%{searchString}%' OR M_ACC.CompanyCode LIKE '%{searchString}%' OR M_ACC.Remarks LIKE '%{searchString}%') AND M_ACC.CompanyId<>0 ORDER BY M_ACC.CompanyName ");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Company,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmCompany",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = 0
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CompanyViewModel> GetCompanyByIdAsync(short CompanyId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CompanyViewModel>($"SELECT M_ACC.CompanyId,M_ACC.CompanyCode,M_ACC.CompanyName,M_ACC.Remarks,M_ACC.IsActive,M_ACC.CreateById,M_ACC.CreateDate,M_ACC.EditById,M_ACC.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.AdmCompany M_ACC  LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACC.EditById WHERE M_ACC.CompanyId={CompanyId} AND M_ACC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Admin},{(short)E_Admin.Company}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Company,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmCompany",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = 0,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCompanyAsync(short UserId, AdmCompany m_Company)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Company.CompanyId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.AdmCompany WHERE CompanyId<>@CompanyId AND CompanyCode=@CompanyCode",
                        new { m_Company.CompanyId, m_Company.CompanyCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Company Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.AdmCompany WHERE CompanyId<>@CompanyId AND CompanyName=@CompanyName",
                        new { m_Company.CompanyId, m_Company.CompanyName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Company Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.AdmCompany WHERE CompanyId=@CompanyId",
                            new { m_Company.CompanyId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Company);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Company Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (CompanyId + 1) FROM dbo.AdmCompany WHERE (CompanyId + 1) NOT IN (SELECT CompanyId FROM dbo.AdmCompany)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Company.CompanyId = Convert.ToByte(sqlMissingResponse.NextId);
                            _context.Add(m_Company);
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
                            CompanyId = 0,
                            ModuleId = (short)E_Modules.Admin,
                            TransactionId = (short)E_Admin.Company,
                            DocumentId = m_Company.CompanyId,
                            DocumentNo = m_Company.CompanyCode,
                            TblName = "AdmCompany",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Company Save Successfully",
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
                catch (SqlException sqlEx)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = 0,
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.Company,
                        DocumentId = m_Company.CompanyId,
                        DocumentNo = m_Company.CompanyCode,
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
                        CompanyId = 0,
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.Company,
                        DocumentId = m_Company.CompanyId,
                        DocumentNo = m_Company.CompanyCode,
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
        }
    }
}