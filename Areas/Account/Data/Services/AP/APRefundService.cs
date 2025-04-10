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
    public sealed class APRefundService : IAPRefundService
    {
        private readonly IRepository<ApRefundHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public APRefundService(IRepository<ApRefundHd> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<APRefundViewModelCount> GetAPRefundListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId)
        {
            APRefundViewModelCount countViewModel = new APRefundViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.ApRefundHd Rpthd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Rpthd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById WHERE (Rpthd.RefundNo LIKE '%{searchString}%' OR Rpthd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.SupplierCode LIKE '%{searchString}%' OR M_Cus.SupplierName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND TrnDate BETWEEN '{fromDate}' AND '{toDate}' AND Rpthd.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<APRefundViewModel>($"SELECT Rpthd.CompanyId,Rpthd.RefundId,Rpthd.RefundNo,Rpthd.ReferenceNo,Rpthd.TrnDate,Rpthd.AccountDate,Rpthd.BankId,M_Ban.BankCode,M_Ban.BankName,Rpthd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Rpthd.ChequeNo,Rpthd.ChequeDate,Rpthd.SupplierId,M_Cus.SupplierCode,M_Cus.SupplierName,Rpthd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Rpthd.ExhRate,Rpthd.TotAmt,Rpthd.TotLocalAmt,Rpthd.RecCurrencyId,R_Cur.CurrencyCode RecCurrencyCode,R_Cur.CurrencyName RecCurrencyName,Rpthd.RecExhRate,Rpthd.RecTotAmt,Rpthd.RecTotLocalAmt,Rpthd.ExhGainLoss,Rpthd.BankChargeGLId,M_chr.GLCode BankChargeGLCode,M_chr.GLName BankChargeGLName,Rpthd.BankChargesAmt,Rpthd.BankChargesLocalAmt,Rpthd.ModuleFrom,Rpthd.CreateById,Rpthd.CreateDate,Rpthd.EditById,Rpthd.EditDate,Rpthd.CancelById,Rpthd.CancelDate,Rpthd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Rpthd.EditVersion FROM dbo.ApRefundHd Rpthd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Rpthd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById LEFT JOIN dbo.M_ChartOfAccount M_chr ON M_chr.GLId = Rpthd.BankChargeGLId WHERE (Rpthd.RefundNo LIKE '%{searchString}%' OR Rpthd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.SupplierCode LIKE '%{searchString}%' OR M_Cus.SupplierName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND Rpthd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Rpthd.CompanyId={CompanyId} ORDER BY Rpthd.RefundNo,Rpthd.AccountDate OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<APRefundViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AR.Refund,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "ApRefundHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<APRefundViewModel> GetAPRefundByIdAsync(short CompanyId, long RefundId, string RefundNo, short UserId)
        {
            APRefundViewModel APRefundViewModel = new APRefundViewModel();
            try
            {
                APRefundViewModel = await _repository.GetQuerySingleOrDefaultAsync<APRefundViewModel>($"SELECT Rpthd.CompanyId,Rpthd.RefundId,Rpthd.RefundNo,Rpthd.ReferenceNo,Rpthd.TrnDate,Rpthd.AccountDate,Rpthd.BankId,M_Ban.BankCode,M_Ban.BankName,Rpthd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Rpthd.ChequeNo,Rpthd.ChequeDate,Rpthd.SupplierId,M_Cus.SupplierCode,M_Cus.SupplierName,Rpthd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Rpthd.ExhRate,Rpthd.TotAmt,Rpthd.TotLocalAmt,Rpthd.RecCurrencyId,R_Cur.CurrencyCode RecCurrencyCode,R_Cur.CurrencyName RecCurrencyName,Rpthd.RecExhRate,Rpthd.RecTotAmt,Rpthd.RecTotLocalAmt,Rpthd.ExhGainLoss,Rpthd.BankChargeGLId,M_chr.GLCode BankChargeGLCode,M_chr.GLName BankChargeGLName,Rpthd.BankChargesAmt,Rpthd.BankChargesLocalAmt,Rpthd.ModuleFrom,Rpthd.CreateById,Rpthd.CreateDate,Rpthd.EditById,Rpthd.EditDate,Rpthd.CancelById,Rpthd.CancelDate,Rpthd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Rpthd.EditVersion FROM dbo.ApRefundHd Rpthd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Rpthd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById LEFT JOIN dbo.M_ChartOfAccount M_chr ON M_chr.GLId = Rpthd.BankChargeGLId WHERE (Rpthd.RefundId={RefundId} OR {RefundId}=0) AND (Rpthd.RefundNo='{RefundNo}' OR '{RefundNo}'='{string.Empty}')");

                if (APRefundViewModel == null)
                    return APRefundViewModel;

                var result = await _repository.GetQueryAsync<APRefundDtViewModel>($"SELECT Rptdt.CompanyId,Rptdt.RefundId,Rptdt.RefundNo,Rptdt.ItemNo,Rptdt.TransactionId,Rptdt.DocumentId,Rptdt.DocumentNo,Rptdt.ReferenceNo,Rptdt.DocCurrencyId,Rptdt.DocExhRate,Rptdt.DocAccountDate,Rptdt.DocDueDate,Rptdt.DocTotAmt,Rptdt.DocTotLocalAmt,Rptdt.DocBalAmt,Rptdt.DocBalLocalAmt,Rptdt.AllocAmt,Rptdt.AllocLocalAmt,Rptdt.DocAllocAmt,Rptdt.DocAllocLocalAmt,Rptdt.CentDiff,Rptdt.ExhGainLoss FROM dbo.ApRefundDt Rptdt  WHERE Rptdt.RefundId={APRefundViewModel.RefundId}");

                APRefundViewModel.data_details = result == null ? null : result.ToList();

                return APRefundViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AR.Refund,
                    DocumentId = RefundId,
                    DocumentNo = RefundNo,
                    TblName = "ApRefundHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveAPRefundAsync(short CompanyId, ApRefundHd arRefundHd, List<ApRefundDt> arRefundDts, short UserId)
        {
            bool IsEdit = false;
            string accountDate = arRefundHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //var IsPeriodClosed = await _repository.GetQuerySingleOrDefaultAsync<bool>( $"SELECT dbo.CheckPeriodClosed({CompanyId},{(short)E_Modules.AP},'{accountDate}') as IsExist");

                    //if (!IsPeriodClosed)
                    //{
                    if (arRefundHd.RefundId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.ApRefundHd WHERE IsCancel=0 And CompanyId={CompanyId} And RefundId={arRefundHd.RefundId}");

                        if (dataExist.Count() == 0)
                            return new SqlResponce { Result = -1, Message = "Refund Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponceIds>($"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.AP},{(short)E_AR.Refund},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            arRefundHd.RefundId = documentIdNo.ToList()[0].DocumentId;
                            arRefundHd.RefundNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_CreateHistoryRec {CompanyId},{UserId},{arRefundHd.RefundId},{(short)E_AR.Refund}");
                    }

                    if (IsEdit)
                    {
                        var entityHead = _context.Update(arRefundHd);
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
                        arRefundHd.EditDate = null;
                        arRefundHd.EditById = null;

                        var entityHead = _context.Add(arRefundHd);
                    }

                    var SaveHeader = _context.SaveChanges();

                    if (SaveHeader > 0)
                    {
                        var SaveDetails = 0;

                        if (IsEdit)
                            _context.ApRefundDt.Where(x => x.RefundId == arRefundHd.RefundId).ExecuteDelete();

                        foreach (var item in arRefundDts)
                        {
                            item.RefundId = arRefundHd.RefundId;
                            item.RefundNo = arRefundHd.RefundNo;
                            _context.Add(item);
                        }

                        if (arRefundDts == null)
                            SaveDetails = _context.SaveChanges();
                        else
                            SaveDetails = 1;

                        #region Save AuditLog

                        if (SaveDetails > 0)
                        {
                            //Inserting the records into AR CreateStatement
                            await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arRefundHd.RefundId},{(short)E_AR.Refund}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AP,
                                TransactionId = (short)E_AR.Refund,
                                DocumentId = arRefundHd.RefundId,
                                DocumentNo = arRefundHd.RefundNo,
                                TblName = "ApRefundHd",
                                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                                Remarks = arRefundHd.Remarks,
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                //Update Edit Version
                                if (IsEdit)
                                    await _repository.UpsertExecuteScalarAsync($"update ApRefundHd set EditVersion=EditVersion+1 where RefundId={arRefundHd.RefundId}; Update ApRefundDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.ApRefundHd where RefundId={arRefundHd.RefundId}) where RefundId={arRefundHd.RefundId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arRefundHd.RefundId},{(short)E_AR.Refund}");

                                TScope.Complete();
                                return new SqlResponce { Result = arRefundHd.RefundId, Message = "Save Successfully" };
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
                    TransactionId = (short)E_AR.Refund,
                    DocumentId = arRefundHd.RefundId,
                    DocumentNo = arRefundHd.RefundNo,
                    TblName = "ApRefundHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponce> DeleteAPRefundAsync(short CompanyId, long RefundId, string RefundNo, string CanacelRemarks, short UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (RefundId > 0)
                    {
                        var APRefundToRemove = _context.ApRefundHd.Where(b => b.RefundId == RefundId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (APRefundToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_DeleteStatement {CompanyId},{UserId},{RefundId},{(short)E_AR.Refund}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AP,
                                TransactionId = (short)E_AR.Refund,
                                DocumentId = RefundId,
                                DocumentNo = RefundNo,
                                TblName = "ApRefundHd",
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
                        return new SqlResponce { Result = -1, Message = "Refund Not exists" };
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
                    TransactionId = (short)E_AR.Refund,
                    DocumentId = RefundId,
                    DocumentNo = RefundNo,
                    TblName = "ApRefundHd",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<APRefundViewModel>> GetHistoryAPRefundByIdAsync(short CompanyId, long RefundId, string RefundNo, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<APRefundViewModel>($"SELECT Rpthd.CompanyId,Rpthd.RefundId,Rpthd.RefundNo,Rpthd.ReferenceNo,Rpthd.TrnDate,Rpthd.AccountDate,Rpthd.BankId,M_Ban.BankCode,M_Ban.BankName,Rpthd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Rpthd.ChequeNo,Rpthd.ChequeDate,Rpthd.SupplierId,M_Cus.SupplierCode,M_Cus.SupplierName,Rpthd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Rpthd.ExhRate,Rpthd.TotAmt,Rpthd.TotLocalAmt,Rpthd.UnAllocTotAmt,Rpthd.UnAllocTotLocalAmt,Rpthd.RecCurrencyId,R_Cur.CurrencyCode RecCurrencyCode,R_Cur.CurrencyName RecCurrencyName,Rpthd.RecExhRate,Rpthd.RecTotAmt,Rpthd.RecTotLocalAmt,Rpthd.ExhGainLoss,Rpthd.AllocTotAmt,Rpthd.AllocTotLocalAmt,Rpthd.BankChargeGLId,M_chr.GLCode BankChargeGLCode,M_chr.GLName BankChargeGLName,Rpthd.BankChargesAmt,Rpthd.BankChargesLocalAmt,Rpthd.ModuleFrom,Rpthd.CreateById,Rpthd.CreateDate,Rpthd.EditById,Rpthd.EditDate,Rpthd.CancelById,Rpthd.CancelDate,Rpthd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Rpthd.EditVersion FROM dbo.ApRefundHd_Ver Rpthd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Rpthd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Rpthd.CurrencyId INNER JOIN dbo.M_Currency R_Cur ON R_Cur.CurrencyId = Rpthd.RecCurrencyId INNER JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Rpthd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Rpthd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Rpthd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Rpthd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Rpthd.CancelById LEFT JOIN dbo.M_ChartOfAccount M_chr ON M_chr.GLId = Rpthd.BankChargeGLId WHERE (Rpthd.RefundId={RefundId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AR.Refund,
                    DocumentId = RefundId,
                    DocumentNo = RefundNo,
                    TblName = "ApRefundHd",
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