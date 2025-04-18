﻿using AMESWEB.Areas.Master.Data.IServices;
using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Entities.Masters;
using AMESWEB.Enums;
using AMESWEB.Helpers;
using AMESWEB.IServices;
using AMESWEB.Models;
using AMESWEB.Models.Masters;
using AMESWEB.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AMESWEB.Areas.Master.Data.Services
{
    public sealed class VoyageService : IVoyageService
    {
        private readonly IRepository<M_Voyage> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public VoyageService(IRepository<M_Voyage> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<VoyageViewModelCount> GetVoyageListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            VoyageViewModelCount countViewModel = new VoyageViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Voyage M_Voy INNER JOIN dbo.M_Vessel M_Ves ON M_Ves.VesselId = M_Voy.VesselId INNER JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = M_Voy.BargeId WHERE (M_Voy.ReferenceNo LIKE '%{searchString}%' OR M_Voy.VoyageNo LIKE '%{searchString}%' OR M_Voy.Remarks LIKE '%{searchString}%' OR M_Ves.VesselName LIKE '%{searchString}%' OR M_Ves.VesselCode LIKE '%{searchString}%' OR M_Bar.BargeName LIKE '%{searchString}%' OR M_Bar.BargeCode LIKE '%{searchString}%') AND M_Voy.VoyageId<>0 AND M_Voy.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Voyage}))");

                var result = await _repository.GetQueryAsync<VoyageViewModel>($"SELECT M_Voy.CompanyId,M_Voy.VoyageId,M_Voy.VoyageNo,M_Voy.ReferenceNo,M_Voy.VesselId,M_Ves.VesselCode,M_Ves.VesselName,M_Voy.BargeId,M_Bar.BargeName,M_Bar.BargeCode,M_Voy.Remarks,M_Voy.IsActive,M_Voy.CreateById,M_Voy.CreateDate,M_Voy.EditById,M_Voy.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Voyage M_Voy INNER JOIN dbo.M_Vessel M_Ves ON M_Ves.VesselId = M_Voy.VesselId INNER JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = M_Voy.BargeId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Voy.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Voy.EditById WHERE (M_Voy.ReferenceNo LIKE '%{searchString}%' OR M_Voy.VoyageNo LIKE '%{searchString}%' OR M_Voy.Remarks LIKE '%{searchString}%' OR M_Ves.VesselName LIKE '%{searchString}%' OR M_Ves.VesselCode LIKE '%{searchString}%' OR M_Bar.BargeName LIKE '%{searchString}%' OR M_Bar.BargeCode LIKE '%{searchString}%') AND M_Voy.VoyageId<>0 AND M_Voy.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Voyage})) ORDER BY M_Voy.VoyageNo OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<VoyageViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Voyage,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Voyage",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<VoyageViewModel> GetVoyageByIdAsync(short CompanyId, short UserId, short VoyageId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<VoyageViewModel>($"SELECT M_Voy.CompanyId,M_Voy.VoyageId,M_Voy.VoyageNo,M_Voy.ReferenceNo,M_Voy.VesselId,M_Ves.VesselCode,M_Ves.VesselName,M_Voy.BargeId,M_Bar.BargeName,M_Bar.BargeCode,M_Voy.Remarks,M_Voy.IsActive,M_Voy.CreateById,M_Voy.CreateDate,M_Voy.EditById,M_Voy.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Voyage M_Voy INNER JOIN dbo.M_Vessel M_Ves ON M_Ves.VesselId = M_Voy.VesselId INNER JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = M_Voy.BargeId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Voy.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Voy.EditById WHERE M_Voy.VoyageId={VoyageId} AND M_Voy.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Voyage}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Voyage,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Voyage",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveVoyageAsync(short CompanyId, short UserId, M_Voyage m_Voyage)
        {
            bool IsEdit = false;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_Voyage WHERE VoyageId<>@VoyageId AND VoyageNo=@VoyageNo", new { m_Voyage.VoyageId, m_Voyage.VoyageNo });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "Voyage No. already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_Voyage WHERE VoyageId<>@VoyageId AND ReferenceNo=@ReferenceNo", new { m_Voyage.VoyageId, m_Voyage.ReferenceNo });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "Reference No. already exists." };

                    if (m_Voyage.VoyageId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_Voyage WHERE VoyageId<>0 AND VoyageId={m_Voyage.VoyageId} ");

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Voyage);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Voyage Not Found" };
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (VoyageId + 1) FROM dbo.M_Voyage WHERE (VoyageId + 1) NOT IN (SELECT VoyageId FROM dbo.M_Voyage)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Voyage.VoyageId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Voyage);
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Voyage,
                            DocumentId = m_Voyage.VoyageId,
                            DocumentNo = m_Voyage.VoyageNo,
                            TblName = "M_Voyage",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Voyage Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponce { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponce();
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Voyage, m_Voyage.VoyageId, m_Voyage.VoyageNo, "M_Voyage", IsEdit ? E_Mode.Update : E_Mode.Create, "SQL", UserId);
                return new SqlResponce { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Voyage, m_Voyage.VoyageId, m_Voyage.VoyageNo, "M_Voyage", IsEdit ? E_Mode.Update : E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> DeleteVoyageAsync(short CompanyId, short UserId, short voyageId)
        {
            string voyageNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    voyageNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT VoyageCode FROM dbo.M_Voyage WHERE VoyageId={voyageId}");

                    if (voyageId > 0)
                    {
                        var accountGroupToRemove = _context.M_Voyage
                            .Where(x => x.VoyageId == voyageId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Voyage,
                                DocumentId = voyageId,
                                DocumentNo = voyageNo,
                                TblName = "M_Voyage",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Voyage Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponce { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "VoyageId Should be zero" };
                    }
                    return new SqlResponce();
                }
            }
            catch (SqlException sqlEx)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Voyage,
                    DocumentId = voyageId,
                    DocumentNo = "",
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    TransactionId = (short)E_Master.Voyage,
                    DocumentId = voyageId,
                    DocumentNo = "",
                    TblName = "M_Voyage",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
    }
}