using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Master.Data.Services
{
    public sealed class GstService : IGstService
    {
        private readonly IRepository<M_Gst> _repository;
        private ApplicationDbContext _context;

        public GstService(IRepository<M_Gst> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        #region GST_HD

        public async Task<GstViewModelCount> GetGstListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString)
        {
            GstViewModelCount countViewModel = new GstViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Gst M_Gt INNER JOIN dbo.M_GstCategory M_gtc ON M_gtc.GstCategoryId = M_Gt.GstCategoryId WHERE (M_Gt.GstName LIKE '%{searchString}%' OR M_Gt.GstCode LIKE '%{searchString}%' OR M_Gt.Remarks LIKE '%{searchString}%' OR M_gtc.GstCategoryName LIKE '%{searchString}%' OR M_gtc.GstCategoryCode LIKE '%{searchString}%') AND M_Gt.GstId<>0 AND M_Gt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstCategory}))");

                var result = await _repository.GetQueryAsync<GstViewModel>($"SELECT M_Gt.GstId,M_Gt.GstCode,M_Gt.GstName,M_Gt.CompanyId,M_Gt.Remarks,M_Gt.IsActive,M_gtc.GstCategoryCode,M_gtc.GstCategoryName,M_Gt.CreateById,M_Gt.CreateDate,M_Gt.EditById,M_Gt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Gst M_Gt INNER JOIN dbo.M_GstCategory M_gtc ON M_gtc.GstCategoryId = M_Gt.GstCategoryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Gt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Gt.EditById WHERE (M_Gt.GstName LIKE '%{searchString}%' OR M_Gt.GstCode LIKE '%{searchString}%' OR M_Gt.Remarks LIKE '%{searchString}%' OR M_gtc.GstCategoryName LIKE '%{searchString}%' OR M_gtc.GstCategoryCode LIKE '%{searchString}%') AND M_Gt.GstId<>0 AND M_Gt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Gst})) ORDER BY M_Gt.GstName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Gst,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Gst",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Gst> GetGstByIdAsync(short CompanyId, short UserId, short GstId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Gst>($"SELECT M_Gt.GstId,M_Gt.GstCode,M_Gt.GstName,M_Gt.CompanyId,M_Gt.Remarks,M_Gt.IsActive,M_gtc.GstCategoryCode,M_gtc.GstCategoryName,M_Gt.CreateById,M_Gt.CreateDate,M_Gt.EditById,M_Gt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Gst M_Gt INNER JOIN dbo.M_GstCategory M_gtc ON M_gtc.GstCategoryId = M_Gt.GstCategoryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Gt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Gt.EditById  WHERE GstId={GstId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Gst}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Gst,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Gst",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveGstAsync(short CompanyId, short UserId, M_Gst Gst)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = Gst.GstId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Gst WHERE GstId<>@GstId AND GstCode=@GstCode AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId))",
                        new { Gst.GstId, Gst.GstCode, CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.Gst });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Gst Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Gst WHERE GstId<>@GstId AND GstName=@GstName AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId))",
                        new { Gst.GstId, Gst.GstName, CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.Gst });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -2, Message = "Gst Name already exists." };

                    if (!IsEdit)
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (GstId + 1) FROM dbo.M_Gst WHERE (GstId + 1) NOT IN (SELECT GstId FROM dbo.M_Gst)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            Gst.GstId = Convert.ToInt16(sqlMissingResponse.NextId);
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "GstId Should not be zero" };
                        }
                    }

                    var entity = IsEdit ? _context.Update(Gst) : _context.Add(Gst);
                    entity.Property(b => b.EditDate).IsModified = false;

                    var GstToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (GstToSave > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Gst,
                            DocumentId = Gst.GstId,
                            DocumentNo = Gst.GstCode,
                            TblName = "M_Gst",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Gst Save Successfully",
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
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Gst,
                        DocumentId = Gst.GstId,
                        DocumentNo = Gst.GstCode,
                        TblName = "M_Gst",
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

        public async Task<SqlResponse> DeleteGstAsync(short CompanyId, short UserId, M_Gst Gst)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (Gst.GstId > 0)
                    {
                        var GstToRemove = _context.M_Gst.Where(x => x.GstId == Gst.GstId).ExecuteDelete();

                        if (GstToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Gst,
                                DocumentId = Gst.GstId,
                                DocumentNo = Gst.GstCode,
                                TblName = "M_Gst",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Gst Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "GstId Should be zero" };
                    }
                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Gst,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Gst",
                        ModeId = (short)E_Mode.Delete,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId,
                    };

                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        #endregion GST_HD

        #region GST_DT

        public async Task<GstDtViewModelCount> GetGstDtListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString)
        {
            GstDtViewModelCount GstDtViewModelCount = new GstDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.M_GstDt M_GsDt INNER JOIN dbo.M_Gst M_Gt ON M_Gt.GstId = M_GsDt.GstId WHERE (M_Gt.GstName LIKE '%{searchString}%' OR M_Gt.GstCode LIKE '%{searchString}%') AND M_GsDt.GstId<>0 AND M_GsDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstDt}))");

                var result = await _repository.GetQueryAsync<GstDtViewModel>($"SELECT M_GsDt.GstId,M_Gt.GstCode,M_Gt.GstName,M_GsDt.GstPercentahge,M_GsDt.CompanyId,M_GsDt.ValidFrom,M_GsDt.CreateById,M_GsDt.CreateDate,M_GsDt.EditById,M_GsDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditB FROM dbo.M_GstDt M_GsDt INNER JOIN dbo.M_Gst M_Gt ON M_Gt.GstId = M_GsDt.GstId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_GsDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_GsDt.EditById WHERE (M_Gt.GstName LIKE '%{searchString}%' OR M_Gt.GstCode LIKE '%{searchString}%') AND M_GsDt.GstId<>0 AND M_GsDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstDt})) ORDER BY M_Gt.GstName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                GstDtViewModelCount.responseCode = 200;
                GstDtViewModelCount.responseMessage = "success";
                GstDtViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                GstDtViewModelCount.data = result == null ? null : result.ToList();

                return GstDtViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GstDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GstDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<GstDtViewModel> GetGstDtByIdAsync(short CompanyId, short UserId, short GstId, DateTime ValidFrom)
        {
            string validFrom = ValidFrom.ToString("yyyy-MM-dd");
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<GstDtViewModel>($"SELECT M_GsDt.GstId,M_Gt.GstCode,M_Gt.GstName,M_GsDt.GstPercentahge,M_GsDt.CompanyId,M_GsDt.ValidFrom,M_GsDt.CreateById,M_GsDt.CreateDate,M_GsDt.EditById,M_GsDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditB FROM dbo.M_GstDt M_GsDt INNER JOIN dbo.M_Gst M_Gt ON M_Gt.GstId = M_GsDt.GstId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_GsDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_GsDt.EditById WHERE M_GsDt.GstId={GstId} AND M_GsDt.ValidFrom = '{validFrom}' AND M_GsDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstDt}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GstDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GstDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveGstDtAsync(short CompanyId, short UserId, M_GstDt m_GstDt)
        {
            string validFrom = m_GstDt.ValidFrom.ToString("yyyy-MM-dd");

            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_GstDt.GstId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_GstDt WHERE CompanyId={CompanyId} AND GstId={m_GstDt.GstId} AND ValidFrom='{validFrom}'");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_GstDt);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            var entityHead = _context.Add(m_GstDt);
                            entityHead.Property(b => b.EditDate).IsModified = false;
                            entityHead.Property(b => b.EditById).IsModified = false;
                        }
                    }

                    var CurrencyDtToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (CurrencyDtToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.GstDt,
                            DocumentId = m_GstDt.GstId,
                            DocumentNo = "",
                            TblName = "M_CurrencyDt",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CurrencyDt Save Successfully",
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
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.GstDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CurrencyDt",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponse> DeleteGstDtAsync(short CompanyId, short UserId, GstDtViewModel m_GstDt)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (m_GstDt.GstId > 0)
                    {
                        var GstDtToRemove = await _context.M_GstDt.Where(x => x.GstId == m_GstDt.GstId && x.ValidFrom == m_GstDt.ValidFrom && x.CompanyId == CompanyId).ExecuteDeleteAsync();

                        if (GstDtToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.GstDt,
                                DocumentId = m_GstDt.GstId,
                                DocumentNo = "",
                                TblName = "M_GstDt",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "GstDt Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "GstId Should be zero" };
                    }
                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.GstDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_GstDt",
                        ModeId = (short)E_Mode.Delete,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId,
                    };

                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        #endregion GST_DT
    }
}