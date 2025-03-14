using AEMSWEB.Areas.Account.Data.IServices;
using AEMSWEB.Areas.Account.Data.IServices.CB;
using AEMSWEB.Areas.Account.Models.CB;
using AEMSWEB.Data;
using AEMSWEB.Entities.Accounts.CB;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Account.Data.Services.CB
{
    public sealed class CBBankTransferService : ICBBankTransferService
    {
        private readonly IRepository<CBBankTransfer> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public CBBankTransferService(IRepository<CBBankTransfer> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<CBBankTransferViewModelList> GetCBBankTransferListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, short UserId)
        {
            CBBankTransferViewModelList countViewModel = new CBBankTransferViewModelList();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.CBBankTransfer Cbbank INNER JOIN dbo.M_Bank frmbank ON frmbank.BankId = Cbbank.FromBankId INNER JOIN dbo.M_Bank tobank ON tobank.BankId = Cbbank.ToBankId INNER JOIN dbo.M_Currency frmcurr ON frmcurr.CurrencyId = Cbbank.FromCurrencyId INNER JOIN dbo.M_Currency tocurr ON tocurr.CurrencyId = Cbbank.ToCurrencyId INNER JOIN dbo.M_PaymentType M_papty ON M_papty.PaymentTypeId = Cbbank.PaymentTypeId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Cbbank.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Cbbank.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Cbbank.CancelById WHERE ( Cbbank.TransferNo LIKE '%{searchString}%' OR  Cbbank.ReferenceNo LIKE '%{searchString}%' OR  frmcurr.CurrencyCode LIKE '%{searchString}%' OR  tocurr.CurrencyCode LIKE '%{searchString}%' OR  frmcurr.CurrencyName LIKE '%{searchString}%' OR  tocurr.CurrencyName LIKE '%{searchString}%' OR  frmbank.BankName LIKE '%{searchString}%' OR  tobank.BankName LIKE '%{searchString}%' OR  frmbank.BankCode LIKE '%{searchString}%' OR  tobank.BankCode LIKE '%{searchString}%' )");
                var result = await _repository.GetQueryAsync<CBBankTransferViewModel>($"SELECT Cbbank.CompanyId,Cbbank.TransferId,Cbbank.TransferNo,Cbbank.ReferenceNo,Cbbank.AccountDate,Cbbank.FromBankId,Cbbank.FromCurrencyId,frmcurr.CurrencyName,frmcurr.CurrencyCode,Cbbank.FromExhRate,Cbbank.PaymentTypeId,M_papty.PaymentTypeCode,M_papty.PaymentTypeName,Cbbank.ChequeNo,Cbbank.ChequeDate,Cbbank.FromBankChgAmt,Cbbank.FromBankChgLocalAmt,Cbbank.FromTotAmt,Cbbank.FromTotLocalAmt,Cbbank.ToBankId,Cbbank.ToCurrencyId,tocurr.CurrencyName,tocurr.CurrencyCode,Cbbank.ToExhRate,Cbbank.ToExhRate,Cbbank.ToBankChgAmt,Cbbank.ToBankChgLocalAmt,Cbbank.ToTotAmt,Cbbank.ToTotLocalAmt,Cbbank.BankExhRate,Cbbank.BankTotAmt,Cbbank.BankTotLocalAmt,Cbbank.Remarks,Cbbank.PayeeTo,Cbbank.ExhGainLoss,Cbbank.ModuleFrom,Cbbank.CreateById,Cbbank.CreateDate,Cbbank.EditById,Cbbank.EditDate,Cbbank.IsCancel,Cbbank.CancelById,Cbbank.CancelDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.CBBankTransfer Cbbank INNER JOIN dbo.M_Bank frmbank ON frmbank.BankId = Cbbank.FromBankId INNER JOIN dbo.M_Bank tobank ON tobank.BankId = Cbbank.ToBankId INNER JOIN dbo.M_Currency frmcurr ON frmcurr.CurrencyId = Cbbank.FromCurrencyId INNER JOIN dbo.M_Currency tocurr ON tocurr.CurrencyId = Cbbank.ToCurrencyId INNER JOIN dbo.M_PaymentType M_papty ON M_papty.PaymentTypeId = Cbbank.PaymentTypeId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Cbbank.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Cbbank.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Cbbank.CancelById WHERE ( Cbbank.TransferNo LIKE '%{searchString}%' OR  Cbbank.ReferenceNo LIKE '%{searchString}%' OR  frmcurr.CurrencyCode LIKE '%{searchString}%' OR  tocurr.CurrencyCode LIKE '%{searchString}%' OR  frmcurr.CurrencyName LIKE '%{searchString}%' OR  tocurr.CurrencyName LIKE '%{searchString}%' OR  frmbank.BankName LIKE '%{searchString}%' OR  tobank.BankName LIKE '%{searchString}%' OR  frmbank.BankCode LIKE '%{searchString}%' OR  tobank.BankCode LIKE '%{searchString}%' ) ORDER BY Cbbank.AccountDate Desc,Cbbank.TransferNo Desc OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result == null ? null : result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBBankTransfer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "CBBankTransfer",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CBBankTransferViewModel> GetCBBankTransferByIdNoAsync(short CompanyId, long TransferId, string TransferNo, short UserId)
        {
            CBBankTransferViewModel CBBankTransferViewModelList = new CBBankTransferViewModel();
            try
            {
                CBBankTransferViewModelList = await _repository.GetQuerySingleOrDefaultAsync<CBBankTransferViewModel>($"SELECT Cbbank.CompanyId,Cbbank.TransferId,Cbbank.TransferNo,Cbbank.ReferenceNo,Cbbank.AccountDate,Cbbank.FromBankId,Cbbank.FromCurrencyId,frmcurr.CurrencyName,frmcurr.CurrencyCode,Cbbank.FromExhRate,Cbbank.PaymentTypeId,M_papty.PaymentTypeCode,M_papty.PaymentTypeName,Cbbank.ChequeNo,Cbbank.ChequeDate,Cbbank.FromBankChgAmt,Cbbank.FromBankChgLocalAmt,Cbbank.FromTotAmt,Cbbank.FromTotLocalAmt,Cbbank.ToBankId,Cbbank.ToCurrencyId,tocurr.CurrencyName,tocurr.CurrencyCode,Cbbank.ToExhRate,Cbbank.ToExhRate,Cbbank.ToBankChgAmt,Cbbank.ToBankChgLocalAmt,Cbbank.ToTotAmt,Cbbank.ToTotLocalAmt,Cbbank.BankExhRate,Cbbank.BankTotAmt,Cbbank.BankTotLocalAmt,Cbbank.Remarks,Cbbank.PayeeTo,Cbbank.ExhGainLoss,Cbbank.ModuleFrom,Cbbank.CreateById,Cbbank.CreateDate,Cbbank.EditById,Cbbank.EditDate,Cbbank.IsCancel,Cbbank.CancelById,Cbbank.CancelDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.CBBankTransfer Cbbank INNER JOIN dbo.M_Bank frmbank ON frmbank.BankId = Cbbank.FromBankId INNER JOIN dbo.M_Bank tobank ON tobank.BankId = Cbbank.ToBankId INNER JOIN dbo.M_Currency frmcurr ON frmcurr.CurrencyId = Cbbank.FromCurrencyId INNER JOIN dbo.M_Currency tocurr ON tocurr.CurrencyId = Cbbank.ToCurrencyId INNER JOIN dbo.M_PaymentType M_papty ON M_papty.PaymentTypeId = Cbbank.PaymentTypeId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Cbbank.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Cbbank.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Cbbank.CancelById WHERE (Cbbank.TransferId={TransferId} OR {TransferId}=0) AND (Cbbank.TransferNo='{TransferNo}' OR '{TransferNo}'='{string.Empty}')");

                if (CBBankTransferViewModelList == null)
                    return CBBankTransferViewModelList;

                return CBBankTransferViewModelList;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBBankTransfer,
                    DocumentId = TransferId,
                    DocumentNo = TransferNo,
                    TblName = "CBBankTransfer",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCBBankTransferAsync(short CompanyId, CBBankTransfer CBBankTransfer, short UserId)
        {
            bool IsEdit = false;
            string accountDate = CBBankTransfer.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (CBBankTransfer.TransferId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.CBBankTransfer WHERE IsCancel=0 And CompanyId={CompanyId} And TransferId={CBBankTransfer.TransferId}");

                        if (dataExist.Count() == 0)
                            return new SqlResponse { Result = -1, Message = "Invoice Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponseIds>($"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.CB},{(short)E_CB.CBBankTransfer},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            CBBankTransfer.TransferId = documentIdNo.ToList()[0].DocumentId;
                            CBBankTransfer.TransferNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_CB_CreateHistoryRec {CompanyId},{UserId},{CBBankTransfer.TransferId},{(short)E_CB.CBBankTransfer}");
                    }

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(CBBankTransfer);
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
                        CBBankTransfer.EditDate = null;
                        CBBankTransfer.EditById = null;

                        var entityHead = _context.Add(CBBankTransfer);

                        entityHead.Property(b => b.IsCancel).IsModified = false;
                        entityHead.Property(b => b.CancelById).IsModified = false;
                        entityHead.Property(b => b.CancelDate).IsModified = false;
                        entityHead.Property(b => b.CancelRemarks).IsModified = false;
                    }

                    var SaveHeader = _context.SaveChanges();

                    //Saving Details
                    if (SaveHeader > 0)
                    {
                        #region Save AuditLog

                        //Inserting the records into AR CreateStatement
                        await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_CB_PosttoGL {CompanyId},{UserId},{CBBankTransfer.TransferId},{(short)E_CB.CBBankTransfer}");

                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.CB,
                            TransactionId = (short)E_CB.CBBankTransfer,
                            DocumentId = CBBankTransfer.TransferId,
                            DocumentNo = CBBankTransfer.TransferNo,
                            TblName = "CBBankTransfer",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = CBBankTransfer.Remarks,
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            //Update Edit Version
                            if (IsEdit)
                                await _repository.UpsertExecuteScalarAsync($"update CBBankTransfer set EditVersion=EditVersion+1 where TransferId={CBBankTransfer.TransferId}");

                            //Create / Update Ar Statement
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_CB_PosttoGL {CompanyId},{UserId},{CBBankTransfer.TransferId},{(short)E_CB.CBBankTransfer}");

                            TScope.Complete();
                            return new SqlResponse { Result = CBBankTransfer.TransferId, Message = "Save Successfully" };
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
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBBankTransfer,
                    DocumentId = CBBankTransfer.TransferId,
                    DocumentNo = CBBankTransfer.TransferNo,
                    TblName = "CBBankTransfer",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponse> DeleteCBBankTransferAsync(short CompanyId, long TransferId, string TransferNo, string CanacelRemarks, short UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Get Invoice Number
                    TransferNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT TransferNo FROM dbo.CBBankTransfer WHERE TransferId={TransferId}");

                    if (TransferId > 0)
                    {
                        //Update IsCancle=1,Cancelby=userid,Canceldate=now,CancelRemarks=CancelRemarks
                        var CBBankTransferToRemove = _context.CBBankTransfer.Where(b => b.TransferId == TransferId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (CBBankTransferToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AR_DeleteStatement {CompanyId},{UserId},{TransferId},{(short)E_CB.CBBankTransfer}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.CB,
                                TransactionId = (short)E_CB.CBBankTransfer,
                                DocumentId = TransferId,
                                DocumentNo = TransferNo,
                                TblName = "CBBankTransfer",
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
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBBankTransfer,
                    DocumentId = TransferId,
                    DocumentNo = TransferNo,
                    TblName = "CBBankTransfer",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CBBankTransferViewModel>> GetHistoryCBBankTransferByIdAsync(short CompanyId, long TransferId, string TransferNo, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<CBBankTransferViewModel>($"SELECT Cbbank.CompanyId,Cbbank.TransferId,Cbbank.TransferNo,Cbbank.ReferenceNo,Cbbank.AccountDate,Cbbank.FromBankId,Cbbank.FromCurrencyId,frmcurr.CurrencyName,frmcurr.CurrencyCode,Cbbank.FromExhRate,Cbbank.PaymentTypeId,M_papty.PaymentTypeCode,M_papty.PaymentTypeName,Cbbank.ChequeNo,Cbbank.ChequeDate,Cbbank.FromBankChgAmt,Cbbank.FromBankChgLocalAmt,Cbbank.FromTotAmt,Cbbank.FromTotLocalAmt,Cbbank.ToBankId,Cbbank.ToCurrencyId,tocurr.CurrencyName,tocurr.CurrencyCode,Cbbank.ToExhRate,Cbbank.ToExhRate,Cbbank.ToBankChgAmt,Cbbank.ToBankChgLocalAmt,Cbbank.ToTotAmt,Cbbank.ToTotLocalAmt,Cbbank.BankExhRate,Cbbank.BankTotAmt,Cbbank.BankTotLocalAmt,Cbbank.Remarks,Cbbank.PayeeTo,Cbbank.ExhGainLoss,Cbbank.ModuleFrom,Cbbank.CreateById,Cbbank.CreateDate,Cbbank.EditById,Cbbank.EditDate,Cbbank.IsCancel,Cbbank.CancelById,Cbbank.CancelDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.CBBankTransfer_Ver Cbbank INNER JOIN dbo.M_Bank frmbank ON frmbank.BankId = Cbbank.FromBankId INNER JOIN dbo.M_Bank tobank ON tobank.BankId = Cbbank.ToBankId INNER JOIN dbo.M_Currency frmcurr ON frmcurr.CurrencyId = Cbbank.FromCurrencyId INNER JOIN dbo.M_Currency tocurr ON tocurr.CurrencyId = Cbbank.ToCurrencyId INNER JOIN dbo.M_PaymentType M_papty ON M_papty.PaymentTypeId = Cbbank.PaymentTypeId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Cbbank.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Cbbank.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Cbbank.CancelById WHERE (Cbbank.TransferId={TransferId} OR {TransferId}=0) AND (Cbbank.TransferNo='{TransferNo}' OR '{TransferNo}'='{string.Empty}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBBankTransfer,
                    DocumentId = TransferId,
                    DocumentNo = TransferNo,
                    TblName = "CBBankTransfer",
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