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
    public sealed class UomService : IUomService
    {
        private readonly IRepository<M_Uom> _repository;
        private ApplicationDbContext _context;

        public UomService(IRepository<M_Uom> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        #region Header

        public async Task<UomViewModelCount> GetUomListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            UomViewModelCount countViewModel = new UomViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Uom M_Um WHERE (M_Um.UomName LIKE '%{searchString}%' OR M_Um.UomCode LIKE '%{searchString}%' OR M_Um.Remarks LIKE '%{searchString}%') AND M_Um.UomId<>0 AND M_Um.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Uom}))");

                var result = await _repository.GetQueryAsync<UomViewModel>($"SELECT M_Um.UomId,M_Um.CompanyId,M_Um.UomCode,M_Um.UomName,M_Um.Remarks,M_Um.IsActive,M_Um.CreateById,M_Um.CreateDate,M_Um.EditById,M_Um.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Uom M_Um LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Um.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Um.EditById WHERE (M_Um.UomName LIKE '%{searchString}%' OR M_Um.UomCode LIKE '%{searchString}%' OR M_Um.Remarks LIKE '%{searchString}%') AND M_Um.UomId<>0 AND M_Um.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Uom})) ORDER BY M_Um.UomName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<UomViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Uom,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Uom",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Uom> GetUomByIdAsync(short CompanyId, short UserId, short UomId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Uom>($"SELECT UomId,CompanyId,UomCode,UomName,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Uom WHERE UomId={UomId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Uom,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Uom",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveUomAsync(short CompanyId, short UserId, M_Uom m_Uom)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Uom.UomId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Uom WHERE UomId<>@UomId AND UomCode=@UomCode",
                        new { m_Uom.UomId, m_Uom.UomCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "UOM Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Uom WHERE UomId<>@UomId AND UomName=@UomName",
                        new { m_Uom.UomId, m_Uom.UomName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "UOM Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Uom WHERE UomId=@UomId",
                            new { m_Uom.UomId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Uom);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "UOM Not Found" };
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (UomId + 1) FROM dbo.M_Uom WHERE (UomId + 1) NOT IN (SELECT UomId FROM dbo.M_Uom)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Uom.UomId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Uom);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Internal Server Error" };
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Uom,
                            DocumentId = m_Uom.UomId,
                            DocumentNo = m_Uom.UomCode,
                            TblName = "M_Uom",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "UOM Save Successfully",
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
                        TransactionId = (short)E_Master.Uom,
                        DocumentId = m_Uom.UomId,
                        DocumentNo = m_Uom.UomCode,
                        TblName = "M_UOM",
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

        public async Task<SqlResponse> DeleteUomAsync(short CompanyId, short UserId, M_Uom Uom)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (Uom.UomId > 0)
                    {
                        var UomToRemove = _context.M_Uom.Where(x => x.UomId == Uom.UomId).ExecuteDelete();

                        if (UomToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Uom,
                                DocumentId = Uom.UomId,
                                DocumentNo = Uom.UomCode,
                                TblName = "M_Uom",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Uom Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "UomId Should be zero" };
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
                        TransactionId = (short)E_Master.Uom,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Uom",
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

        #endregion Header

        #region Details

        public async Task<UomDtViewModelCount> GetUomDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            UomDtViewModelCount countViewModel = new UomDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.M_UomDt M_UmDt INNER JOIN dbo.M_Uom M_Um ON M_Um.UomId = M_UmDt.UomId WHERE (M_Um.UomName LIKE '%{searchString}%' OR M_Um.UomCode LIKE '%{searchString}%' OR M_Um.Remarks LIKE '%{searchString}%') AND M_Um.UomId<>0 AND M_Um.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.UomDt}))");

                var result = await _repository.GetQueryAsync<UomDtViewModel>($"SELECT M_UmDt.CompanyId,M_UmDt.UomId,M_Um.UomCode,M_Um.UomName,M_UmDt.PackUomId,M_Um.UomCode  PackUomCode,M_Um.UomName PackUomName,M_UmDt.UomFactor,M_UmDt.CreateById,M_UmDt.CreateDate,M_UmDt.EditById,M_UmDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_UomDt M_UmDt INNER JOIN dbo.M_Uom M_Um ON M_Um.UomId = M_UmDt.UomId INNER JOIN dbo.M_Uom M_UmPk ON M_UmPk.UomId = M_UmDt.PackUomId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Um.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Um.EditById WHERE (M_Um.UomName LIKE '%{searchString}%' OR M_Um.UomCode LIKE '%{searchString}%' OR M_Um.Remarks LIKE '%{searchString}%') AND M_Um.UomId<>0 AND M_Um.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.UomDt})) ORDER BY M_Um.UomName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<UomDtViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.UomDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_UomDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<UomDtViewModel> GetUomDtByIdAsync(short CompanyId, short UserId, short UomId, short PackUomId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<UomDtViewModel>($"SELECT M_UmDt.CompanyId,M_UmDt.UomId,M_Um.UomCode,M_Um.UomName,M_UmDt.PackUomId,M_Um.UomCode  PackUomCode,M_Um.UomName PackUomName,M_UmDt.UomFactor,M_UmDt.CreateById,M_UmDt.CreateDate,M_UmDt.EditById,M_UmDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_UomDt M_UmDt INNER JOIN dbo.M_Uom M_Um ON M_Um.UomId = M_UmDt.UomId INNER JOIN dbo.M_Uom M_UmPk ON M_UmPk.UomId = M_UmDt.PackUomId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Um.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Um.EditById WHERE UomId={UomId} AND PackUomId={PackUomId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.UomDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_UomDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveUomDtAsync(short CompanyId, short UserId, M_UomDt m_UomDt)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_UomDt.UomId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_UomDt WHERE CompanyId={CompanyId} AND UomId={m_UomDt.UomId} AND PackUomId={m_UomDt.PackUomId}");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_UomDt);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            var entityHead = _context.Add(m_UomDt);
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
                            TransactionId = (short)E_Master.UomDt,
                            DocumentId = m_UomDt.UomId,
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
                        TransactionId = (short)E_Master.UomDt,
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

        public async Task<SqlResponse> DeleteUomDtAsync(short CompanyId, short UserId, short UomId, short PackUomId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (UomId > 0)
                    {
                        var UomDtToRemove = await _context.M_UomDt.Where(x => x.UomId == UomId && x.PackUomId == PackUomId && x.CompanyId == CompanyId).ExecuteDeleteAsync();

                        if (UomDtToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.UomDt,
                                DocumentId = UomId,
                                DocumentNo = "",
                                TblName = "M_UomDt",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "UomDt Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "UomId Should be zero" };
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
                        TransactionId = (short)E_Master.UomDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_UomDt",
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

        #endregion Details
    }
}