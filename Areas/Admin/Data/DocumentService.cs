using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;
using AEMSWEB.Repository;
using System.Transactions;

namespace AEMSWEB.Areas.Admin.Data
{
    public sealed class DocumentService : IDocumentService
    {
        private readonly IRepository<AdmDocuments> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public DocumentService(IRepository<AdmDocuments> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<IEnumerable<DocumentViewModel>> GetDocumentByIdAsync(short CompanyId, short ModuleId, short TransactionId, long DocumentId, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<DocumentViewModel>($"SELECT AdmDoc.CompanyId,AdmDoc.ModuleId,AdmDoc.TransactionId,AdmDoc.DocumentId,AdmDoc.DocumentNo,AdmDoc.ItemNo,AdmDoc.DocTypeId,M_Doc.DocTypeCode,M_Doc.DocTypeName,AdmDoc.DocPath,AdmDoc.Remarks,AdmDoc.CreateById,AdmDoc.CreateDate,Usr.UserName AS CreateBy,AdmDoc.EditById,AdmDoc.EditDate,Usr1.UserName AS EditBy FROM dbo.AdmDocuments AdmDoc INNER JOIN dbo.M_DocumentType M_Doc ON M_Doc.DocTypeId = AdmDoc.DocTypeId   LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = AdmDoc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = AdmDoc.EditById WHERE AdmDoc.ModuleId={ModuleId} AND AdmDoc.TransactionId={TransactionId} AND AdmDoc.DocumentId={DocumentId} AND AdmDoc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Admin},{(short)E_Admin.Document}))");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Document,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmDocuments",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveDocumentAsync(short CompanyId, AdmDocuments admDocuments, short UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var IsResultExist = await _repository.GetQuerySingleOrDefaultAsync($"SELECT TOP 1 1 AS EXIST FROM dbo.AdmDocuments WHERE CompanyId={CompanyId} AND ModuleId={admDocuments.ModuleId} AND TransactionId={admDocuments.TransactionId} AND DocumentId={admDocuments.DocumentId} AND ItemNo={admDocuments.ItemNo}");

                    if (IsResultExist)
                    {
                        var entityHead = _context.Update(admDocuments);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CompanyId).IsModified = false;
                        entityHead.Property(b => b.ModuleId).IsModified = false;
                        entityHead.Property(b => b.TransactionId).IsModified = false;
                        entityHead.Property(b => b.DocumentId).IsModified = false;
                        entityHead.Property(b => b.DocumentNo).IsModified = false;
                        entityHead.Property(b => b.ItemNo).IsModified = false;
                        entityHead.Property(b => b.DocPath).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT ISNULL((SELECT TOP 1(ItemNo + 1) FROM dbo.AdmDocuments WHERE CompanyId={admDocuments.CompanyId} AND ModuleId={admDocuments.ModuleId} AND TransactionId={admDocuments.TransactionId} AND DocumentId={admDocuments.DocumentId} AND (ItemNo + 1) NOT IN(SELECT ItemNo FROM dbo.AdmDocuments where CompanyId={admDocuments.CompanyId} AND ModuleId={admDocuments.ModuleId} AND TransactionId={admDocuments.TransactionId} AND DocumentId={admDocuments.DocumentId})),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            admDocuments.ItemNo = Convert.ToInt16(sqlMissingResponse.NextId);
                            admDocuments.EditById = null;
                            admDocuments.EditDate = null;
                            _context.Add(admDocuments);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "ItemNo Should not be zero" };
                    }

                    var DocumentToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (DocumentToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Admin,
                            TransactionId = (short)E_Admin.Document,
                            DocumentId = Convert.ToInt64(admDocuments.DocumentId),
                            DocumentNo = admDocuments.DocumentNo,
                            TblName = "AdmDocuments",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "Document Save Successfully",
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
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.Document,
                        DocumentId = Convert.ToInt64(admDocuments.DocumentId),
                        DocumentNo = admDocuments.DocumentNo,
                        TblName = "AdmDocuments",
                        ModeId = (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException?.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponse> DeleteDocumentAsync(short CompanyId, short ModuleId, short TransactionId, long DocumentId, int ItemNo, short UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var IsResultExist = await _repository.GetQuerySingleOrDefaultAsync<AdmDocuments>($"SELECT * FROM dbo.AdmDocuments WHERE CompanyId={CompanyId} AND ModuleId={ModuleId} AND TransactionId={TransactionId} AND DocumentId={DocumentId} AND ItemNo={ItemNo}");

                try
                {
                    if (IsResultExist == null)
                        return new SqlResponse { Result = -1, Message = "Record Not Found" };

                    if (DocumentId <= 0 && ItemNo <= 0)
                        return new SqlResponse { Result = -1, Message = "DocTypeId Should be greater than zero" };

                    var deleteResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.AdmDocuments WHERE CompanyId={CompanyId} AND ModuleId={ModuleId} AND TransactionId={TransactionId} AND DocumentId={DocumentId} AND ItemNo={ItemNo}");

                    if (deleteResult == 0)
                        return new SqlResponse { Result = -1, Message = "No records delete" };
                    else
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Admin,
                            TransactionId = (short)E_Admin.Document,
                            DocumentId = Convert.ToInt64(DocumentId),
                            DocumentNo = IsResultExist.DocumentNo,
                            TblName = "AdmDocuments",
                            ModeId = (short)E_Mode.Delete,
                            Remarks = "Document Delete Successfully",
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
                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.Document,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "AdmDocuments",
                        ModeId = (short)E_Mode.Delete,
                        Remarks = ex.Message + (ex.InnerException?.Message ?? ""),
                        CreateById = UserId,
                    };

                    _context.Add(errorLog);
                    await _context.SaveChangesAsync();

                    throw new Exception(ex.ToString());
                }
            }
        }
    }
}