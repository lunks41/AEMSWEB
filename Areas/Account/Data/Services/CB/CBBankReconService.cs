using AMESWEB.Areas.Account.Data.IServices;
using AMESWEB.Areas.Account.Data.IServices.CB;
using AMESWEB.Areas.Account.Models.CB;
using AMESWEB.Data;
using AMESWEB.Entities.Accounts.CB;
using AMESWEB.Entities.Admin;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Models;
using AMESWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AMESWEB.Areas.Account.Data.Services.CB
{
    public sealed class CBBankReconService : ICBBankReconService
    {
        private readonly IRepository<CBBankReconHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public CBBankReconService(IRepository<CBBankReconHd> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<CBBankReconViewModel> GetCBBankReconListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId)
        {
            CBBankReconViewModel countViewModel = new CBBankReconViewModel();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.CBBankReconHd Invhd INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId  INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ReconNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND Invhd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId}");
                var result = await _repository.GetQueryAsync<CBBankReconHdViewModel>($"SELECT Invhd.CompanyId,Invhd.ReconId,Invhd.ReconNo,Invhd.PrevReconId,Invhd.PrevReconNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.FromDate,Invhd.ToDate,Invhd.Remarks,Invhd.TotAmt,Invhd.OPBalAmt,Invhd.CLBalAmt,Invhd.Version,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.CBBankReconHd Invhd  INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId  INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ReconNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND Invhd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId} ORDER BY Invhd.AccountDate Desc,Invhd.ReconNo Desc OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<CBBankReconHdViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBBankRecon,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "CBBankReconHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CBBankReconHdViewModel> GetCBBankReconByIdNoAsync(short CompanyId, long ReconId, string ReconNo, short UserId)
        {
            CBBankReconHdViewModel CBBankReconHdViewModel = new CBBankReconHdViewModel();
            try
            {
                CBBankReconHdViewModel = await _repository.GetQuerySingleOrDefaultAsync<CBBankReconHdViewModel>($"SELECT Invhd.CompanyId,Invhd.ReconId,Invhd.ReconNo,Invhd.PrevReconId,Invhd.PrevReconNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.FromDate,Invhd.ToDate,Invhd.Remarks,Invhd.TotAmt,Invhd.OPBalAmt,Invhd.CLBalAmt,Invhd.Version,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.CBBankReconHd Invhd  INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId  INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ReconId={ReconId} OR {ReconId}=0) AND (Invhd.ReconNo='{ReconNo}' OR '{ReconNo}'='{string.Empty}')");

                if (CBBankReconHdViewModel == null)
                    return CBBankReconHdViewModel;

                var result = await _repository.GetQueryAsync<CBBankReconDtViewModel>($"SELECT Invdt.ReconId,Invdt.ReconNo,Invdt.ItemNo,Invdt.IsSel,Invdt.ModuleId,Invdt.TransactionId,Invdt.DocumentId,Invdt.DocumentNo,Invdt.DocReferenceNo,Invdt.AccountDate,Invdt.PaymentTypeId,Invdt.ChequeNo,Invdt.ChequeDate,Invdt.CustomerId,Invdt.SupplierId,Invdt.GLId,M_chra.GLCode,M_chra.GLName,Invdt.IsDebit,Invdt.ExhRate,Invdt.TotAmt,Invdt.TotLocalAmt,Invdt.PaymentFromTo,Invdt.Remarks,Invdt.EditVersion FROM dbo.CBBankReconDt Invdt LEFT JOIN dbo.M_ChartOfAccount M_chra ON M_chra.GLId = Invdt.GLId  WHERE Invdt.ReconId={CBBankReconHdViewModel.ReconId}");

                CBBankReconHdViewModel.data_details = result == null ? null : result.ToList();

                return CBBankReconHdViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBBankRecon,
                    DocumentId = ReconId,
                    DocumentNo = ReconNo,
                    TblName = "CBBankReconHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCBBankReconAsync(short CompanyId, CBBankReconHd CBBankReconHd, List<CBBankReconDt> CBBankReconDt, short UserId)
        {
            bool IsEdit = false;
            string accountDate = CBBankReconHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (CBBankReconHd.ReconId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.CBBankReconHd WHERE IsCancel=0 And CompanyId={CompanyId} And ReconId={CBBankReconHd.ReconId}");

                        if (dataExist.Count() == 0)
                            return new SqlResponce { Result = -1, Message = "Invoice Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponceIds>($"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.CB},{(short)E_CB.CBBankRecon},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            CBBankReconHd.ReconId = documentIdNo.ToList()[0].DocumentId;
                            CBBankReconHd.ReconNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_CB_CreateHistoryRec {CompanyId},{UserId},{CBBankReconHd.ReconId},{(short)E_CB.CBBankRecon}");
                    }

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(CBBankReconHd);
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
                        CBBankReconHd.EditDate = null;
                        CBBankReconHd.EditById = null;

                        var entityHead = _context.Add(CBBankReconHd);
                    }

                    var SaveHeader = _context.SaveChanges();

                    //Saving Details
                    if (SaveHeader > 0)
                    {
                        if (IsEdit)
                            _context.CBBankReconDt.Where(x => x.ReconId == CBBankReconHd.ReconId).ExecuteDelete();

                        foreach (var item in CBBankReconDt)
                        {
                            item.ReconId = CBBankReconHd.ReconId;
                            item.ReconNo = CBBankReconHd.ReconNo;
                            _context.Add(item);
                        }

                        var SaveDetails = _context.SaveChanges();

                        #region Save AuditLog

                        if (SaveDetails > 0)
                        {
                            //Inserting the records into AR CreateStatement
                            await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_CB_CreateStatement {CompanyId},{UserId},{CBBankReconHd.ReconId},{(short)E_CB.CBBankRecon}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.CB,
                                TransactionId = (short)E_CB.CBBankRecon,
                                DocumentId = CBBankReconHd.ReconId,
                                DocumentNo = CBBankReconHd.ReconNo,
                                TblName = "CBBankReconHd",
                                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                                Remarks = CBBankReconHd.Remarks,
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                //Update Edit Version
                                if (IsEdit)
                                    await _repository.UpsertExecuteScalarAsync($"update CBBankReconHd set EditVersion=EditVersion+1 where ReconId={CBBankReconHd.ReconId}; Update CBBankReconDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.CBBankReconHd where ReconId={CBBankReconHd.ReconId}) where ReconId={CBBankReconHd.ReconId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_CB_CreateStatement {CompanyId},{UserId},{CBBankReconHd.ReconId},{(short)E_CB.CBBankRecon}");

                                TScope.Complete();
                                return new SqlResponce { Result = CBBankReconHd.ReconId, Message = "Save Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = 1, Message = "Save Failed" };
                        }

                        #endregion Save AuditLog
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "Id Should not be zero" };
                    }

                    return new SqlResponce();
                }
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBBankRecon,
                    DocumentId = CBBankReconHd.ReconId,
                    DocumentNo = CBBankReconHd.ReconNo,
                    TblName = "CBBankReconHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponce> DeleteCBBankReconAsync(short CompanyId, long ReconId, string CanacelRemarks, short UserId)
        {
            string ReconNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Get Invoice Number
                    ReconNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT ReconNo FROM dbo.CBBankReconHd WHERE ReconId={ReconId}");

                    if (ReconId > 0)
                    {
                        //Update IsCancle=1,Cancelby=userid,Canceldate=now,CancelRemarks=CancelRemarks
                        var CBBankReconToRemove = _context.CBBankReconHd.Where(b => b.ReconId == ReconId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (CBBankReconToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_CB_DeleteStatement {CompanyId},{UserId},{ReconId},{(short)E_CB.CBBankRecon}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.CB,
                                TransactionId = (short)E_CB.CBBankRecon,
                                DocumentId = ReconId,
                                DocumentNo = ReconNo,
                                TblName = "CBBankReconHd",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = CanacelRemarks,
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponce { Result = 1, Message = "Cancel Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Cancel Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "Invoice Not exists" };
                    }
                    return new SqlResponce();
                }
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBBankRecon,
                    DocumentId = ReconId,
                    DocumentNo = ReconNo,
                    TblName = "CBBankReconHd",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CBBankReconHdViewModel>> GetHistoryCBBankReconByIdAsync(short CompanyId, long ReconId, string ReconNo, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<CBBankReconHdViewModel>($"SELECT Invhd.EditVersion,Invhd.CompanyId,Invhd.ReconId,Invhd.ReconNo,Invhd.PrevReconId,Invhd.PrevReconNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.FromDate,Invhd.ToDate,Invhd.Remarks,Invhd.TotAmt,Invhd.OPBalAmt,Invhd.CLBalAmt,Invhd.Version,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.CBBankReconHd_Ver Invhd INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId  INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ReconId={ReconId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBBankRecon,
                    DocumentId = ReconId,
                    DocumentNo = ReconNo,
                    TblName = "CBBankReconHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CBBankReconHdViewModel> GetHistoryCBBankReconByIdAsync_V1(short CompanyId, long ReconId, string ReconNo, short UserId)
        {
            CBBankReconHdViewModel CBBankReconHdViewModel = new CBBankReconHdViewModel();
            try
            {
                CBBankReconHdViewModel = await _repository.GetQuerySingleOrDefaultAsync<CBBankReconHdViewModel>($"SELECT Invhd.EditVersion,Invhd.CompanyId,Invhd.ReconId,Invhd.ReconNo,Invhd.PrevReconId,Invhd.PrevReconNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.FromDate,Invhd.ToDate,Invhd.Remarks,Invhd.TotAmt,Invhd.OPBalAmt,Invhd.CLBalAmt,Invhd.Version,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.CBBankReconHd_Ver Invhd INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId  INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ReconId={ReconId} OR {ReconId}=0) AND (Invhd.ReconNo='{ReconNo}' OR '{ReconNo}'='{string.Empty}')");

                if (CBBankReconHdViewModel == null)
                    return CBBankReconHdViewModel;

                var result = await _repository.GetQueryAsync<CBBankReconDtViewModel>($"SELECT Invdt.ReconId,Invdt.ReconNo,Invdt.ItemNo,Invdt.IsSel,Invdt.ModuleId,Invdt.TransactionId,Invdt.DocumentId,Invdt.DocumentNo,Invdt.DocReferenceNo,Invdt.AccountDate,Invdt.PaymentTypeId,Invdt.ChequeNo,Invdt.ChequeDate,Invdt.CustomerId,Invdt.SupplierId,Invdt.GLId,M_chra.GLCode,M_chra.GLName,Invdt.IsDebit,Invdt.ExhRate,Invdt.TotAmt,Invdt.TotLocalAmt,Invdt.PaymentFromTo,Invdt.Remarks,Invdt.EditVersion FROM dbo.CBBankReconDt_Ver Invdt LEFT JOIN dbo.M_ChartOfAccount M_chra ON M_chra.GLId = Invdt.GLId  WHERE Invdt.ReconId={CBBankReconHdViewModel.ReconId}");

                CBBankReconHdViewModel.data_details = result == null ? null : result.ToList();

                return CBBankReconHdViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBBankRecon,
                    DocumentId = ReconId,
                    DocumentNo = ReconNo,
                    TblName = "CBBankReconHd",
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