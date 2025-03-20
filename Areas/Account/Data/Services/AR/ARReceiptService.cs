using AEMSWEB.Areas.Account.Data.IServices;
using AEMSWEB.Areas.Account.Data.IServices.AR;
using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Data;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Account.Data.Services.AR
{
    public sealed class ARReceiptService : IARReceiptService
    {
        private readonly IRepository<ArReceiptHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public ARReceiptService(IRepository<ArReceiptHd> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<ARReceiptViewModelCount> GetARReceiptListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId)
        {
            ARReceiptViewModelCount countViewModel = new ARReceiptViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.ArReceiptHd Rpthd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Rpthd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById WHERE (Rpthd.ReceiptNo LIKE '%{searchString}%' OR Rpthd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND TrnDate BETWEEN '{fromDate}' AND '{toDate}' AND Rpthd.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<ARReceiptViewModel>($"SELECT Rpthd.CompanyId,Rpthd.ReceiptId,Rpthd.ReceiptNo,Rpthd.ReferenceNo,Rpthd.TrnDate,Rpthd.AccountDate,Rpthd.BankId,M_Ban.BankCode,M_Ban.BankName,Rpthd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Rpthd.ChequeNo,Rpthd.ChequeDate,Rpthd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Rpthd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Rpthd.ExhRate,Rpthd.TotAmt,Rpthd.TotLocalAmt,Rpthd.UnAllocTotAmt,Rpthd.UnAllocTotLocalAmt,Rpthd.RecCurrencyId,R_Cur.CurrencyCode RecCurrencyCode,R_Cur.CurrencyName RecCurrencyName,Rpthd.RecExhRate,Rpthd.RecTotAmt,Rpthd.RecTotLocalAmt,Rpthd.ExhGainLoss,Rpthd.AllocTotAmt,Rpthd.AllocTotLocalAmt,Rpthd.BankChargeGLId,M_chr.GLCode BankChargeGLCode,M_chr.GLName BankChargeGLName,Rpthd.BankChargesAmt,Rpthd.BankChargesLocalAmt,Rpthd.ModuleFrom,Rpthd.CreateById,Rpthd.CreateDate,Rpthd.EditById,Rpthd.EditDate,Rpthd.CancelById,Rpthd.CancelDate,Rpthd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Rpthd.EditVersion FROM dbo.ArReceiptHd Rpthd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Rpthd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById LEFT JOIN dbo.M_ChartOfAccount M_chr ON M_chr.GLId = Rpthd.BankChargeGLId WHERE (Rpthd.ReceiptNo LIKE '%{searchString}%' OR Rpthd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND Rpthd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Rpthd.CompanyId={CompanyId} ORDER BY Rpthd.ReceiptNo,Rpthd.AccountDate OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<ARReceiptViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Receipt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "ArReceiptHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ARReceiptViewModel> GetARReceiptByIdAsync(short CompanyId, long ReceiptId, string ReceiptNo, short UserId)
        {
            ARReceiptViewModel ARReceiptViewModel = new ARReceiptViewModel();
            try
            {
                ARReceiptViewModel = await _repository.GetQuerySingleOrDefaultAsync<ARReceiptViewModel>($"SELECT Rpthd.CompanyId,Rpthd.ReceiptId,Rpthd.ReceiptNo,Rpthd.ReferenceNo,Rpthd.TrnDate,Rpthd.AccountDate,Rpthd.BankId,M_Ban.BankCode,M_Ban.BankName,Rpthd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Rpthd.ChequeNo,Rpthd.ChequeDate,Rpthd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Rpthd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Rpthd.ExhRate,Rpthd.TotAmt,Rpthd.TotLocalAmt,Rpthd.UnAllocTotAmt,Rpthd.UnAllocTotLocalAmt,Rpthd.RecCurrencyId,R_Cur.CurrencyCode RecCurrencyCode,R_Cur.CurrencyName RecCurrencyName,Rpthd.RecExhRate,Rpthd.RecTotAmt,Rpthd.RecTotLocalAmt,Rpthd.ExhGainLoss,Rpthd.AllocTotAmt,Rpthd.AllocTotLocalAmt,Rpthd.BankChargeGLId,M_chr.GLCode BankChargeGLCode,M_chr.GLName BankChargeGLName,Rpthd.BankChargesAmt,Rpthd.BankChargesLocalAmt,Rpthd.ModuleFrom,Rpthd.CreateById,Rpthd.CreateDate,Rpthd.EditById,Rpthd.EditDate,Rpthd.CancelById,Rpthd.CancelDate,Rpthd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Rpthd.EditVersion FROM dbo.ArReceiptHd Rpthd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Rpthd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById LEFT JOIN dbo.M_ChartOfAccount M_chr ON M_chr.GLId = Rpthd.BankChargeGLId WHERE (Rpthd.ReceiptId={ReceiptId} OR {ReceiptId}=0) AND (Rpthd.ReceiptNo='{ReceiptNo}' OR '{ReceiptNo}'='{string.Empty}')");

                if (ARReceiptViewModel == null)
                    return ARReceiptViewModel;

                var result = await _repository.GetQueryAsync<ARReceiptDtViewModel>($"SELECT Rptdt.CompanyId,Rptdt.ReceiptId,Rptdt.ReceiptNo,Rptdt.ItemNo,Rptdt.TransactionId,Rptdt.DocumentId,Rptdt.DocumentNo,Rptdt.ReferenceNo,Rptdt.DocCurrencyId,Rptdt.DocExhRate,Rptdt.DocAccountDate,Rptdt.DocDueDate,Rptdt.DocTotAmt,Rptdt.DocTotLocalAmt,Rptdt.DocBalAmt,Rptdt.DocBalLocalAmt,Rptdt.AllocAmt,Rptdt.AllocLocalAmt,Rptdt.DocAllocAmt,Rptdt.DocAllocLocalAmt,Rptdt.CentDiff,Rptdt.ExhGainLoss FROM dbo.ArReceiptDt Rptdt  WHERE Rptdt.ReceiptId={ARReceiptViewModel.ReceiptId}");

                ARReceiptViewModel.data_details = result == null ? null : result.ToList();

                return ARReceiptViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Receipt,
                    DocumentId = ReceiptId,
                    DocumentNo = ReceiptNo,
                    TblName = "ArReceiptHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveARReceiptAsync(short CompanyId, ArReceiptHd arReceiptHd, List<ArReceiptDt> arReceiptDts, short UserId)
        {
            bool IsEdit = false;
            string accountDate = arReceiptHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //var IsPeriodClosed = await _repository.GetQuerySingleOrDefaultAsync<bool>( $"SELECT dbo.CheckPeriodClosed({CompanyId},{(short)E_Modules.AR},'{accountDate}') as IsExist");

                    //if (!IsPeriodClosed)
                    //{
                    if (arReceiptHd.ReceiptId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.ArReceiptHd WHERE IsCancel=0 And CompanyId={CompanyId} And ReceiptId={arReceiptHd.ReceiptId}");

                        if (dataExist.Count() == 0)
                            return new SqlResponse { Result = -1, Message = "Receipt Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponseIds>($"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.AR},{(short)E_AR.Receipt},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            arReceiptHd.ReceiptId = documentIdNo.ToList()[0].DocumentId;
                            arReceiptHd.ReceiptNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        //Insert the previous ARReceipt record to ARReceipt history table as well as editversion also.
                        await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AR_CreateHistoryRec {CompanyId},{UserId},{arReceiptHd.ReceiptId},{(short)E_AR.Receipt}");
                    }

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(arReceiptHd);
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
                        arReceiptHd.EditDate = null;
                        arReceiptHd.EditById = null;

                        var entityHead = _context.Add(arReceiptHd);
                    }

                    var SaveHeader = _context.SaveChanges();

                    //Saving Details
                    if (SaveHeader > 0)
                    {
                        var SaveDetails = 0;

                        if (IsEdit)
                            _context.ArReceiptDt.Where(x => x.ReceiptId == arReceiptHd.ReceiptId).ExecuteDelete();

                        foreach (var item in arReceiptDts)
                        {
                            item.ReceiptId = arReceiptHd.ReceiptId;
                            item.ReceiptNo = arReceiptHd.ReceiptNo;
                            _context.Add(item);
                        }

                        if (arReceiptDts == null)
                            SaveDetails = _context.SaveChanges();
                        else
                            SaveDetails = 1;

                        #region Save AuditLog

                        if (SaveDetails > 0)
                        {
                            //Inserting the records into AR CreateStatement
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arReceiptHd.ReceiptId},{(short)E_AR.Receipt}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AR,
                                TransactionId = (short)E_AR.Receipt,
                                DocumentId = arReceiptHd.ReceiptId,
                                DocumentNo = arReceiptHd.ReceiptNo,
                                TblName = "ArReceiptHd",
                                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                                Remarks = arReceiptHd.Remarks,
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                //Update Edit Version
                                if (IsEdit)
                                    await _repository.UpsertExecuteScalarAsync($"update ArReceiptHd set EditVersion=EditVersion+1 where ReceiptId={arReceiptHd.ReceiptId}; Update ArReceiptDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.ArReceiptHd where ReceiptId={arReceiptHd.ReceiptId}) where ReceiptId={arReceiptHd.ReceiptId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arReceiptHd.ReceiptId},{(short)E_AR.Receipt}");

                                TScope.Complete();
                                return new SqlResponse { Result = arReceiptHd.ReceiptId, Message = "Save Successfully" };
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
                    //}
                    //else
                    //{
                    //    return new SqlResponse { Result = -1, Message = "Period Closed" };
                    //}
                    return new SqlResponse();
                }
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Receipt,
                    DocumentId = arReceiptHd.ReceiptId,
                    DocumentNo = arReceiptHd.ReceiptNo,
                    TblName = "ArReceiptHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponse> DeleteARReceiptAsync(short CompanyId, long ReceiptId, string ReceiptNo, string CanacelRemarks, short UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (ReceiptId > 0)
                    {
                        var ARReceiptToRemove = _context.ArReceiptHd.Where(b => b.ReceiptId == ReceiptId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (ARReceiptToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AR_DeleteStatement {CompanyId},{UserId},{ReceiptId},{(short)E_AR.Receipt}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AR,
                                TransactionId = (short)E_AR.Receipt,
                                DocumentId = ReceiptId,
                                DocumentNo = ReceiptNo,
                                TblName = "ArReceiptHd",
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
                        return new SqlResponse { Result = -1, Message = "Receipt Not exists" };
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
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Receipt,
                    DocumentId = ReceiptId,
                    DocumentNo = ReceiptNo,
                    TblName = "ArReceiptHd",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<ARReceiptViewModel>> GetHistoryARReceiptByIdAsync(short CompanyId, long ReceiptId, string ReceiptNo, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<ARReceiptViewModel>($"SELECT Rpthd.CompanyId,Rpthd.ReceiptId,Rpthd.ReceiptNo,Rpthd.ReferenceNo,Rpthd.TrnDate,Rpthd.AccountDate,Rpthd.BankId,M_Ban.BankCode,M_Ban.BankName,Rpthd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Rpthd.ChequeNo,Rpthd.ChequeDate,Rpthd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Rpthd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Rpthd.ExhRate,Rpthd.TotAmt,Rpthd.TotLocalAmt,Rpthd.UnAllocTotAmt,Rpthd.UnAllocTotLocalAmt,Rpthd.RecCurrencyId,R_Cur.CurrencyCode RecCurrencyCode,R_Cur.CurrencyName RecCurrencyName,Rpthd.RecExhRate,Rpthd.RecTotAmt,Rpthd.RecTotLocalAmt,Rpthd.ExhGainLoss,Rpthd.AllocTotAmt,Rpthd.AllocTotLocalAmt,Rpthd.BankChargeGLId,M_chr.GLCode BankChargeGLCode,M_chr.GLName BankChargeGLName,Rpthd.BankChargesAmt,Rpthd.BankChargesLocalAmt,Rpthd.ModuleFrom,Rpthd.CreateById,Rpthd.CreateDate,Rpthd.EditById,Rpthd.EditDate,Rpthd.CancelById,Rpthd.CancelDate,Rpthd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Rpthd.EditVersion FROM dbo.ArReceiptHd_Ver Rpthd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Rpthd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById LEFT JOIN dbo.M_ChartOfAccount M_chr ON M_chr.GLId = Rpthd.BankChargeGLId WHERE (Rpthd.ReceiptId={ReceiptId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Receipt,
                    DocumentId = ReceiptId,
                    DocumentNo = ReceiptNo,
                    TblName = "ArReceiptHd",
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