﻿using AMESWEB.Areas.Master.Data.IServices;
using AMESWEB.Data;
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
    public sealed class DocumentTypeService : IDocumentTypeService
    {
        private readonly IRepository<M_DocumentType> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public DocumentTypeService(IRepository<M_DocumentType> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<DocumentTypeViewModelCount> GetDocumentTypeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            DocumentTypeViewModelCount countViewModel = new DocumentTypeViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_DocumentType M_Doc WHERE (M_Doc.DocTypeName LIKE '%{searchString}%' OR M_Doc.DocTypeCode LIKE '%{searchString}%' OR M_Doc.Remarks LIKE '%{searchString}%' ) AND M_Doc.DocTypeId<>0 AND M_Doc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.DocumentType}))");

                var result = await _repository.GetQueryAsync<DocumentTypeViewModel>($"SELECT M_Doc.DocTypeId,M_Doc.DocTypeCode,M_Doc.DocTypeName,M_Doc.CompanyId,M_Doc.Remarks,M_Doc.IsActive,M_Doc.CreateById,M_Doc.CreateDate,M_Doc.EditById,M_Doc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_DocumentType M_Doc  LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Doc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Doc.EditById WHERE (M_Doc.DocTypeName LIKE '%{searchString}%' OR M_Doc.DocTypeCode LIKE '%{searchString}%' OR M_Doc.Remarks LIKE '%{searchString}%') AND M_Doc.DocTypeId<>0 AND M_Doc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.DocumentType})) ORDER BY M_Doc.DocTypeName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<DocumentTypeViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.DocumentType, 0, "", "M_DocumentType", E_Mode.View, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<DocumentTypeViewModel> GetDocumentTypeByIdAsync(short CompanyId, short UserId, short DocTypeId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DocumentTypeViewModel>($"SELECT DocTypeId,DocTypeCode,DocTypeName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_DocumentType WHERE DocTypeId={DocTypeId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.DocumentType}))");

                return result;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.DocumentType, 0, "", "M_DocumentType", E_Mode.View, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveDocumentTypeAsync(short CompanyId, short UserId, M_DocumentType DocumentType)
        {
            bool IsEdit = DocumentType.DocTypeId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_DocumentType WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId)) AND DocTypeCode=@DocTypeCode",
                        new { CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.DocumentType, DocumentType.DocTypeCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "DocumentType Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_DocumentType WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId)) AND DocTypeName=@DocTypeName",
                        new { CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.DocumentType, DocumentType.DocTypeName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -2, Message = "DocumentType Name already exists." };

                    // Take the Next Id From SQL
                    var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        "SELECT ISNULL((SELECT TOP 1 (DocTypeId + 1) FROM dbo.M_DocumentType WHERE (DocTypeId + 1) NOT IN (SELECT DocTypeId FROM dbo.M_DocumentType)),1) AS NextId");
                    if (sqlMissingResponse != null)
                    {
                        DocumentType.DocTypeId = Convert.ToInt16(sqlMissingResponse.NextId);

                        var entity = _context.Add(DocumentType);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var DocumentTypeToSave = _context.SaveChanges();

                        if (DocumentTypeToSave > 0)
                        {
                            await _logService.SaveAuditLogAsync(CompanyId, E_Modules.Master, E_Master.DocumentType, DocumentType.DocTypeId, DocumentType.DocTypeCode, "M_DocumentType", IsEdit ? E_Mode.Update : E_Mode.Create, "DocumentType Save Successfully", UserId);
                            TScope.Complete();
                            return new SqlResponce { Result = 1, Message = "Save Successfully" };
                        }
                        else
                        {
                            return new SqlResponce { Result = 1, Message = "Save Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "DocTypeId Should not be zero" };
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.DocumentType, DocumentType.DocTypeId, DocumentType.DocTypeCode, "M_DocumentType", IsEdit ? E_Mode.Update : E_Mode.Create, "SQL", UserId);
                return new SqlResponce { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.DocumentType, DocumentType.DocTypeId, DocumentType.DocTypeCode, "M_DocumentType", IsEdit ? E_Mode.Update : E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> DeleteDocumentTypeAsync(short CompanyId, short UserId, DocumentTypeViewModel documentTypeViewModel)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (documentTypeViewModel.DocTypeId > 0)
                    {
                        var DocumentTypeToRemove = _context.M_DocumentType.Where(x => x.DocTypeId == documentTypeViewModel.DocTypeId).ExecuteDelete();

                        await _logService.SaveAuditLogAsync(CompanyId, E_Modules.Master, E_Master.DocumentType, documentTypeViewModel.DocTypeId, documentTypeViewModel.DocTypeCode, "M_DocumentType", E_Mode.Delete, "DocumentType Delete Successfully", UserId);
                        TScope.Complete();
                        return new SqlResponce { Result = 1, Message = "Delete Successfully" };
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "DocTypeId Should be zero" };
                    }
                }
                catch (SqlException sqlEx)
                {
                    await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.DocumentType, documentTypeViewModel.DocTypeId, documentTypeViewModel.DocTypeCode, "M_DocumentType", E_Mode.Delete, "SQL", UserId);
                    return new SqlResponce { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
                }
                catch (Exception ex)
                {
                    await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.DocumentType, documentTypeViewModel.DocTypeId, documentTypeViewModel.DocTypeCode, "M_DocumentType", E_Mode.Delete, "General", UserId);
                    throw new Exception(ex.ToString());
                }
            }
        }
    }
}