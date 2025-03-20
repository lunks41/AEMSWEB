using AEMSWEB.Areas.Account.Data.IServices;
using AEMSWEB.Areas.Account.Data.IServices.GL;
using AEMSWEB.Areas.Account.Models.GL;
using AEMSWEB.Data;
using AEMSWEB.Entities.Accounts.GL;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Account.Data.Services.GL
{
    public sealed class GLContraService : IGLContraService
    {
        private readonly IRepository<GLContraHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public GLContraService(IRepository<GLContraHd> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<GLContraViewModel> GetGLContraListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId)
        {
            GLContraViewModel countViewModel = new GLContraViewModel();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.GLContraHd Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ContraNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%') AND Invhd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId}");
                var result = await _repository.GetQueryAsync<GLContraHdViewModel>($"SELECT Invhd.CompanyId,Invhd.ContraId,Invhd.ContraNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,Invhd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.Remarks,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.ExhGainLoss,Invhd.ModuleFrom,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.GLContraHd Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ContraNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%') AND Invhd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId} ORDER BY Invhd.AccountDate Desc,Invhd.ContraNo Desc OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<GLContraHdViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.JournalEntry,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GLContraHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<GLContraHdViewModel> GetGLContraByIdNoAsync(short CompanyId, long ContraId, string ContraNo, short UserId)
        {
            GLContraHdViewModel GLContraHdViewModel = new GLContraHdViewModel();
            try
            {
                GLContraHdViewModel = await _repository.GetQuerySingleOrDefaultAsync<GLContraHdViewModel>($"SELECT Invhd.CompanyId,Invhd.ContraId,Invhd.ContraNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,Invhd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.Remarks,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.ExhGainLoss,Invhd.ModuleFrom,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.GLContraHd Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ContraId={ContraId} OR {ContraId}=0) AND (Invhd.ContraNo='{ContraNo}' OR '{ContraNo}'='{string.Empty}')");

                if (GLContraHdViewModel == null)
                    return GLContraHdViewModel;

                var result = await _repository.GetQueryAsync<GLContraDtViewModel>($"SELECT Invdt.ContraId,Invdt.ContraNo,Invdt.ItemNo,Invdt.ModuleId,Invdt.TransactionId,Invdt.DocumentId,Invdt.DocumentNo,Invdt.DocReferenceNo,Invdt.DocCurrencyId,Invdt.DocExhRate,Invdt.DocAccountDate,Invdt.DocDueDate,Invdt.DocTotAmt,Invdt.DocTotLocalAmt,Invdt.DocBalAmt,Invdt.DocBalLocalAmt,Invdt.AllocAmt,Invdt.AllocLocalAmt,Invdt.DocAllocAmt,Invdt.DocAllocLocalAmt,Invdt.CentDiff,Invdt.ExhGainLoss FROM dbo.GLContraDt Invdt WHERE Invdt.ContraId={GLContraHdViewModel.ContraId}");

                GLContraHdViewModel.data_details = result == null ? null : result.ToList();

                return GLContraHdViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.JournalEntry,
                    DocumentId = ContraId,
                    DocumentNo = ContraNo,
                    TblName = "GLContraHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveGLContraAsync(short CompanyId, GLContraHd GLContraHd, List<GLContraDt> GLContraDt, short UserId)
        {
            bool IsEdit = false;
            string accountDate = GLContraHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (GLContraHd.ContraId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.GLContraHd WHERE IsCancel=0 And CompanyId={CompanyId} And ContraId={GLContraHd.ContraId}");

                        if (dataExist.Count() == 0)
                            return new SqlResponse { Result = -1, Message = "Invoice Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponseIds>($"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.GL},{(short)E_GL.JournalEntry},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            GLContraHd.ContraId = documentIdNo.ToList()[0].DocumentId;
                            GLContraHd.ContraNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_GL_CreateHistoryRec {CompanyId},{UserId},{GLContraHd.ContraId},{(short)E_GL.JournalEntry}");
                    }

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(GLContraHd);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CompanyId).IsModified = false;
                        entityHead.Property(b => b.EditVersion).IsModified = false;

                        entityHead.Property(b => b.IsCancel).IsModified = false;
                        entityHead.Property(b => b.CancelById).IsModified = false;
                        entityHead.Property(b => b.CancelDate).IsModified = false;
                        entityHead.Property(b => b.CancelRemarks).IsModified = false;
                    }
                    else
                    {
                        GLContraHd.EditDate = null;
                        GLContraHd.EditById = null;

                        var entityHead = _context.Add(GLContraHd);
                    }

                    var SaveHeader = _context.SaveChanges();

                    //Saving Details
                    if (SaveHeader > 0)
                    {
                        if (IsEdit)
                            _context.GLContraDt.Where(x => x.ContraId == GLContraHd.ContraId).ExecuteDelete();

                        foreach (var item in GLContraDt)
                        {
                            item.ContraId = GLContraHd.ContraId;
                            item.ContraNo = GLContraHd.ContraNo;
                            _context.Add(item);
                        }

                        var SaveDetails = _context.SaveChanges();

                        #region Save AuditLog

                        if (SaveDetails > 0)
                        {
                            //Inserting the records into CB CreateStatement
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_GL_PosttoGL {CompanyId},{UserId},{GLContraHd.ContraId},{(short)E_GL.JournalEntry}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.GL,
                                TransactionId = (short)E_GL.JournalEntry,
                                DocumentId = GLContraHd.ContraId,
                                DocumentNo = GLContraHd.ContraNo,
                                TblName = "GLContraHd",
                                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                                Remarks = GLContraHd.Remarks,
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                //Update Edit Version
                                if (IsEdit)
                                    await _repository.UpsertExecuteScalarAsync($"update GLContraHd set EditVersion=EditVersion+1 where ContraId={GLContraHd.ContraId}; Update GLContraDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.GLContraHd where ContraId={GLContraHd.ContraId}) where ContraId={GLContraHd.ContraId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_GL_PosttoGL {CompanyId},{UserId},{GLContraHd.ContraId},{(short)E_GL.JournalEntry}");

                                TScope.Complete();
                                return new SqlResponse { Result = GLContraHd.ContraId, Message = "Save Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = 1, Message = "Save Failed" };
                        }

                        #endregion Save AuditLog
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "Id Should not be zero" };
                    }

                    return new SqlResponse();
                }
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.JournalEntry,
                    DocumentId = GLContraHd.ContraId,
                    DocumentNo = GLContraHd.ContraNo,
                    TblName = "GLContraHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponse> DeleteGLContraAsync(short CompanyId, long ContraId, string CanacelRemarks, short UserId)
        {
            string ContraNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Get Invoice Number
                    ContraNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT ContraNo FROM dbo.GLContraHd WHERE ContraId={ContraId}");

                    if (ContraId > 0)
                    {
                        //Update IsCancle=1,Cancelby=userid,Canceldate=now,CancelRemarks=CancelRemarks
                        var GLContraToRemove = _context.GLContraHd.Where(b => b.ContraId == ContraId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (GLContraToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_CB_DeleteStatement {CompanyId},{UserId},{ContraId},{(short)E_GL.JournalEntry}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.GL,
                                TransactionId = (short)E_GL.JournalEntry,
                                DocumentId = ContraId,
                                DocumentNo = ContraNo,
                                TblName = "GLContraHd",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = CanacelRemarks,
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Cancel Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Cancel Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "Invoice Not exists" };
                    }
                    return new SqlResponse();
                }
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.JournalEntry,
                    DocumentId = ContraId,
                    DocumentNo = ContraNo,
                    TblName = "GLContraHd",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<GLContraHdViewModel>> GetHistoryGLContraByIdAsync(short CompanyId, long ContraId, string ContraNo, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<GLContraHdViewModel>($"SELECT Invhd.CompanyId,Invhd.ContraId,Invhd.ContraNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,Invhd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.Remarks,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.ExhGainLoss,Invhd.ModuleFrom,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.GLContraHd_Ver Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ContraId={ContraId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.JournalEntry,
                    DocumentId = ContraId,
                    DocumentNo = ContraNo,
                    TblName = "GLContraHd",
                    ModeId = (short)E_Mode.View,
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