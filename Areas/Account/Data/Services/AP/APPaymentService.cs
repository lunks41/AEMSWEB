using AMESWEB.Areas.Account.Data.IServices;
using AMESWEB.Areas.Account.Data.IServices.AP;
using AMESWEB.Areas.Account.Models.AP;
using AMESWEB.Data;
using AMESWEB.Entities.Accounts.AP;
using AMESWEB.Entities.Admin;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Models;
using AMESWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AMESWEB.Areas.Account.Data.Services.AP
{
    public sealed class APPaymentService : IAPPaymentService
    {
        private readonly IRepository<ApPaymentHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public APPaymentService(IRepository<ApPaymentHd> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<APPaymentViewModelCount> GetAPPaymentListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId)
        {
            APPaymentViewModelCount countViewModel = new APPaymentViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.ApPaymentHd Rpthd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Rpthd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById WHERE (Rpthd.PaymentNo LIKE '%{searchString}%' OR Rpthd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.SupplierCode LIKE '%{searchString}%' OR M_Cus.SupplierName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND TrnDate BETWEEN '{fromDate}' AND '{toDate}' AND Rpthd.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<APPaymentViewModel>($"SELECT Rpthd.CompanyId,Rpthd.PaymentId,Rpthd.PaymentNo,Rpthd.ReferenceNo,Rpthd.TrnDate,Rpthd.AccountDate,Rpthd.BankId,M_Ban.BankCode,M_Ban.BankName,Rpthd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Rpthd.ChequeNo,Rpthd.ChequeDate,Rpthd.SupplierId,M_Cus.SupplierCode,M_Cus.SupplierName,Rpthd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Rpthd.ExhRate,Rpthd.TotAmt,Rpthd.TotLocalAmt,Rpthd.UnAllocTotAmt,Rpthd.UnAllocTotLocalAmt,Rpthd.RecCurrencyId,R_Cur.CurrencyCode RecCurrencyCode,R_Cur.CurrencyName RecCurrencyName,Rpthd.RecExhRate,Rpthd.RecTotAmt,Rpthd.RecTotLocalAmt,Rpthd.ExhGainLoss,Rpthd.AllocTotAmt,Rpthd.AllocTotLocalAmt,Rpthd.BankChargeGLId,M_chr.GLCode BankChargeGLCode,M_chr.GLName BankChargeGLName,Rpthd.BankChargesAmt,Rpthd.BankChargesLocalAmt,Rpthd.ModuleFrom,Rpthd.CreateById,Rpthd.CreateDate,Rpthd.EditById,Rpthd.EditDate,Rpthd.CancelById,Rpthd.CancelDate,Rpthd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Rpthd.EditVersion FROM dbo.ApPaymentHd Rpthd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Rpthd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById LEFT JOIN dbo.M_ChartOfAccount M_chr ON M_chr.GLId = Rpthd.BankChargeGLId WHERE (Rpthd.PaymentNo LIKE '%{searchString}%' OR Rpthd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.SupplierCode LIKE '%{searchString}%' OR M_Cus.SupplierName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND Rpthd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Rpthd.CompanyId={CompanyId} ORDER BY Rpthd.PaymentNo,Rpthd.AccountDate OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<APPaymentViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.Payment,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "ApPaymentHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<APPaymentViewModel> GetAPPaymentByIdAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId)
        {
            APPaymentViewModel APPaymentViewModel = new APPaymentViewModel();
            try
            {
                APPaymentViewModel = await _repository.GetQuerySingleOrDefaultAsync<APPaymentViewModel>($"SELECT Rpthd.CompanyId,Rpthd.PaymentId,Rpthd.PaymentNo,Rpthd.ReferenceNo,Rpthd.TrnDate,Rpthd.AccountDate,Rpthd.BankId,M_Ban.BankCode,M_Ban.BankName,Rpthd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Rpthd.ChequeNo,Rpthd.ChequeDate,Rpthd.SupplierId,M_Cus.SupplierCode,M_Cus.SupplierName,Rpthd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Rpthd.ExhRate,Rpthd.TotAmt,Rpthd.TotLocalAmt,Rpthd.UnAllocTotAmt,Rpthd.UnAllocTotLocalAmt,Rpthd.RecCurrencyId,R_Cur.CurrencyCode RecCurrencyCode,R_Cur.CurrencyName RecCurrencyName,Rpthd.RecExhRate,Rpthd.RecTotAmt,Rpthd.RecTotLocalAmt,Rpthd.ExhGainLoss,Rpthd.AllocTotAmt,Rpthd.AllocTotLocalAmt,Rpthd.BankChargeGLId,M_chr.GLCode BankChargeGLCode,M_chr.GLName BankChargeGLName,Rpthd.BankChargesAmt,Rpthd.BankChargesLocalAmt,Rpthd.ModuleFrom,Rpthd.CreateById,Rpthd.CreateDate,Rpthd.EditById,Rpthd.EditDate,Rpthd.CancelById,Rpthd.CancelDate,Rpthd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Rpthd.EditVersion FROM dbo.ApPaymentHd Rpthd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Rpthd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById LEFT JOIN dbo.M_ChartOfAccount M_chr ON M_chr.GLId = Rpthd.BankChargeGLId WHERE (Rpthd.PaymentId={PaymentId} OR {PaymentId}=0) AND (Rpthd.PaymentNo='{PaymentNo}' OR '{PaymentNo}'='{string.Empty}')");

                if (APPaymentViewModel == null)
                    return APPaymentViewModel;

                var result = await _repository.GetQueryAsync<APPaymentDtViewModel>($"SELECT Rptdt.CompanyId,Rptdt.PaymentId,Rptdt.PaymentNo,Rptdt.ItemNo,Rptdt.TransactionId,Rptdt.DocumentId,Rptdt.DocumentNo,Rptdt.ReferenceNo,Rptdt.DocCurrencyId,Rptdt.DocExhRate,Rptdt.DocAccountDate,Rptdt.DocDueDate,Rptdt.DocTotAmt,Rptdt.DocTotLocalAmt,Rptdt.DocBalAmt,Rptdt.DocBalLocalAmt,Rptdt.AllocAmt,Rptdt.AllocLocalAmt,Rptdt.DocAllocAmt,Rptdt.DocAllocLocalAmt,Rptdt.CentDiff,Rptdt.ExhGainLoss FROM dbo.ApPaymentDt Rptdt  WHERE Rptdt.PaymentId={APPaymentViewModel.PaymentId}");

                APPaymentViewModel.data_details = result == null ? null : result.ToList();

                return APPaymentViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.Payment,
                    DocumentId = PaymentId,
                    DocumentNo = PaymentNo,
                    TblName = "ApPaymentHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveAPPaymentAsync(short CompanyId, ApPaymentHd arPaymentHd, List<ApPaymentDt> arPaymentDts, short UserId)
        {
            bool IsEdit = false;
            string accountDate = arPaymentHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (arPaymentHd.PaymentId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.ApPaymentHd WHERE IsCancel=0 And CompanyId={CompanyId} And PaymentId={arPaymentHd.PaymentId}");

                        if (dataExist.Count() == 0)
                            return new SqlResponce { Result = -1, Message = "Payment Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponceIds>($"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.AP},{(short)E_AP.Payment},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            arPaymentHd.PaymentId = documentIdNo.ToList()[0].DocumentId;
                            arPaymentHd.PaymentNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        //Insert the previous APPayment record to APPayment history table as well as editversion also.
                        await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_CreateHistoryRec {CompanyId},{UserId},{arPaymentHd.PaymentId},{(short)E_AP.Payment}");
                    }

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(arPaymentHd);
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
                        arPaymentHd.EditDate = null;
                        arPaymentHd.EditById = null;

                        var entityHead = _context.Add(arPaymentHd);
                    }

                    var SaveHeader = _context.SaveChanges();

                    //Saving Details
                    if (SaveHeader > 0)
                    {
                        var SaveDetails = 0;

                        if (IsEdit)
                            _context.ApPaymentDt.Where(x => x.PaymentId == arPaymentHd.PaymentId).ExecuteDelete();

                        foreach (var item in arPaymentDts)
                        {
                            item.PaymentId = arPaymentHd.PaymentId;
                            item.PaymentNo = arPaymentHd.PaymentNo;
                            _context.Add(item);
                        }

                        if (arPaymentDts == null)
                            SaveDetails = _context.SaveChanges();
                        else
                            SaveDetails = 1;

                        #region Save AuditLog

                        if (SaveDetails > 0)
                        {
                            //Inserting the records into AR CreateStatement
                            await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arPaymentHd.PaymentId},{(short)E_AP.Payment}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AP,
                                TransactionId = (short)E_AP.Payment,
                                DocumentId = arPaymentHd.PaymentId,
                                DocumentNo = arPaymentHd.PaymentNo,
                                TblName = "ApPaymentHd",
                                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                                Remarks = arPaymentHd.Remarks,
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                //Update Edit Version
                                if (IsEdit)
                                    await _repository.UpsertExecuteScalarAsync($"update ApPaymentHd set EditVersion=EditVersion+1 where PaymentId={arPaymentHd.PaymentId}; Update ApPaymentDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.ApPaymentHd where PaymentId={arPaymentHd.PaymentId}) where PaymentId={arPaymentHd.PaymentId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arPaymentHd.PaymentId},{(short)E_AP.Payment}");

                                TScope.Complete();
                                return new SqlResponce { Result = arPaymentHd.PaymentId, Message = "Save Successfully" };
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
                    //}
                    //else
                    //{
                    //    return new SqlResponce { Result = -1, Message = "Period Closed" };
                    //}
                    return new SqlResponce();
                }
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.Payment,
                    DocumentId = arPaymentHd.PaymentId,
                    DocumentNo = arPaymentHd.PaymentNo,
                    TblName = "ApPaymentHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponce> DeleteAPPaymentAsync(short CompanyId, long PaymentId, string PaymentNo, string CanacelRemarks, short UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (PaymentId > 0)
                    {
                        var APPaymentToRemove = _context.ApPaymentHd.Where(b => b.PaymentId == PaymentId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (APPaymentToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_DeleteStatement {CompanyId},{UserId},{PaymentId},{(short)E_AP.Payment}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AP,
                                TransactionId = (short)E_AP.Payment,
                                DocumentId = PaymentId,
                                DocumentNo = PaymentNo,
                                TblName = "ApPaymentHd",
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
                        return new SqlResponce { Result = -1, Message = "Payment Not exists" };
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
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.Payment,
                    DocumentId = PaymentId,
                    DocumentNo = PaymentNo,
                    TblName = "ApPaymentHd",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<APPaymentViewModel>> GetHistoryAPPaymentByIdAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<APPaymentViewModel>($"SELECT Rpthd.CompanyId,Rpthd.PaymentId,Rpthd.PaymentNo,Rpthd.ReferenceNo,Rpthd.TrnDate,Rpthd.AccountDate,Rpthd.BankId,M_Ban.BankCode,M_Ban.BankName,Rpthd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Rpthd.ChequeNo,Rpthd.ChequeDate,Rpthd.SupplierId,M_Cus.SupplierCode,M_Cus.SupplierName,Rpthd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Rpthd.ExhRate,Rpthd.TotAmt,Rpthd.TotLocalAmt,Rpthd.UnAllocTotAmt,Rpthd.UnAllocTotLocalAmt,Rpthd.RecCurrencyId,R_Cur.CurrencyCode RecCurrencyCode,R_Cur.CurrencyName RecCurrencyName,Rpthd.RecExhRate,Rpthd.RecTotAmt,Rpthd.RecTotLocalAmt,Rpthd.ExhGainLoss,Rpthd.AllocTotAmt,Rpthd.AllocTotLocalAmt,Rpthd.BankChargeGLId,M_chr.GLCode BankChargeGLCode,M_chr.GLName BankChargeGLName,Rpthd.BankChargesAmt,Rpthd.BankChargesLocalAmt,Rpthd.ModuleFrom,Rpthd.CreateById,Rpthd.CreateDate,Rpthd.EditById,Rpthd.EditDate,Rpthd.CancelById,Rpthd.CancelDate,Rpthd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Rpthd.EditVersion FROM dbo.ApPaymentHd_Ver Rpthd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Rpthd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById LEFT JOIN dbo.M_ChartOfAccount M_chr ON M_chr.GLId = Rpthd.BankChargeGLId WHERE (Rpthd.PaymentId={PaymentId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.Payment,
                    DocumentId = PaymentId,
                    DocumentNo = PaymentNo,
                    TblName = "ApPaymentHd",
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