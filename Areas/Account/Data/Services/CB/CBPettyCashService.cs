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
    public sealed class CBPettyCashService : ICBPettyCashService
    {
        private readonly IRepository<CBPettyCashHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public CBPettyCashService(IRepository<CBPettyCashHd> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<CBPettyCashViewModel> GetCBPettyCashListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId)
        {
            CBPettyCashViewModel countViewModel = new CBPettyCashViewModel();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.CBPettyCashHd Invhd INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId LEFT JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Invhd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.PaymentNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND Invhd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId}");
                var result = await _repository.GetQueryAsync<CBPettyCashHdViewModel>($"SELECT Invhd.CompanyId,Invhd.PaymentId,Invhd.PaymentNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Invhd.ChequeNo,Invhd.ChequeDate,Invhd.BankChgAmt,Invhd.BankChgLocalAmt,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.Remarks,Invhd.PayeeTo,Invhd.ModuleFrom,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.CBPettyCashHd Invhd INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId LEFT JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Invhd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.PaymentNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND Invhd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId} ORDER BY Invhd.AccountDate Desc,Invhd.PaymentNo Desc OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_CB.CBPattyCash,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "CBPettyCashHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CBPettyCashHdViewModel> GetCBPettyCashByIdNoAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId)
        {
            CBPettyCashHdViewModel CBPettyCashViewModel = new CBPettyCashHdViewModel();
            try
            {
                CBPettyCashViewModel = await _repository.GetQuerySingleOrDefaultAsync<CBPettyCashHdViewModel>($"SELECT Invhd.CompanyId,Invhd.PaymentId,Invhd.PaymentNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Invhd.ChequeNo,Invhd.ChequeDate,Invhd.BankChgAmt,Invhd.BankChgLocalAmt,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.Remarks,Invhd.PayeeTo,Invhd.ModuleFrom,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.CBPettyCashHd Invhd INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId LEFT JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Invhd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.PaymentId={PaymentId} OR {PaymentId}=0) AND (Invhd.PaymentNo='{PaymentNo}' OR '{PaymentNo}'='{string.Empty}')");

                if (CBPettyCashViewModel == null)
                    return CBPettyCashViewModel;

                var result = await _repository.GetQueryAsync<CBPettyCashDtViewModel>($"SELECT Invdt.PaymentId,Invdt.PaymentNo,Invdt.ItemNo,Invdt.SeqNo,Invdt.GLId,M_chra.GLCode,M_chra.GLName,Invdt.TotAmt,Invdt.TotLocalAmt,Invdt.TotCtyAmt,Invdt.Remarks,Invdt.GstId,M_Gs.GstCode,M_Gs.GstName,Invdt.GstPercentage,Invdt.GstAmt,Invdt.GstLocalAmt,Invdt.GstCtyAmt,Invdt.DepartmentId,M_Dep.DepartmentCode,M_Dep.DepartmentName,Invdt.EmployeeId,M_Emp.EmployeeCode,M_Emp.EmployeeName,Invdt.PortId,M_Po.PortCode,M_Po.PortName,Invdt.VesselId,M_Vel.VesselCode,M_Vel.VesselName,Invdt.BargeId,M_Brg.BargeCode,M_Brg.BargeName,Invdt.VoyageId,M_Vo.VoyageNo,M_Vo.ReferenceNo as VoyageReferenceNo,Invdt.EditVersion FROM dbo.CBPettyCashDt Invdt  LEFT JOIN dbo.M_ChartOfAccount M_chra ON M_chra.GLId = Invdt.GLId LEFT JOIN dbo.M_Gst M_Gs ON M_Gs.GstId = Invdt.GstId LEFT JOIN dbo.M_Department M_Dep ON M_Dep.DepartmentId = Invdt.DepartmentId LEFT JOIN dbo.M_Employee M_Emp ON M_Emp.EmployeeId = Invdt.EmployeeId LEFT JOIN dbo.M_Port M_Po ON M_Po.PortId = Invdt.PortId LEFT JOIN dbo.M_Vessel M_Vel ON M_Vel.VesselId = Invdt.VesselId LEFT JOIN dbo.M_Barge M_Brg ON M_Brg.BargeId = Invdt.BargeId LEFT JOIN dbo.M_Voyage M_Vo ON M_Vo.VoyageId = Invdt.VoyageId   WHERE Invdt.PaymentId={CBPettyCashViewModel.PaymentId}");

                CBPettyCashViewModel.data_details = result == null ? null : result.ToList();

                return CBPettyCashViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBPattyCash,
                    DocumentId = PaymentId,
                    DocumentNo = PaymentNo,
                    TblName = "CBPettyCashHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCBPettyCashAsync(short CompanyId, CBPettyCashHd CBPettyCashHd, List<CBPettyCashDt> CBPettyCashDt, short UserId)
        {
            bool IsEdit = false;
            string accountDate = CBPettyCashHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (CBPettyCashHd.PaymentId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.CBPettyCashHd WHERE IsCancel=0 And CompanyId={CompanyId} And PaymentId={CBPettyCashHd.PaymentId}");

                        if (dataExist.Count() == 0)
                            return new SqlResponse { Result = -1, Message = "Invoice Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponseIds>($"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.CB},{(short)E_CB.CBPattyCash},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            CBPettyCashHd.PaymentId = documentIdNo.ToList()[0].DocumentId;
                            CBPettyCashHd.PaymentNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_CB_CreateHistoryRec {CompanyId},{UserId},{CBPettyCashHd.PaymentId},{(short)E_CB.CBPattyCash}");
                    }

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(CBPettyCashHd);
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
                        CBPettyCashHd.EditDate = null;
                        CBPettyCashHd.EditById = null;

                        var entityHead = _context.Add(CBPettyCashHd);
                    }

                    var SaveHeader = _context.SaveChanges();

                    //Saving Details
                    if (SaveHeader > 0)
                    {
                        if (IsEdit)
                            _context.CBPettyCashDt.Where(x => x.PaymentId == CBPettyCashHd.PaymentId).ExecuteDelete();

                        foreach (var item in CBPettyCashDt)
                        {
                            item.PaymentId = CBPettyCashHd.PaymentId;
                            item.PaymentNo = CBPettyCashHd.PaymentNo;
                            _context.Add(item);
                        }

                        var SaveDetails = _context.SaveChanges();

                        #region Save AuditLog

                        if (SaveDetails > 0)
                        {
                            //Inserting the records into AR CreateStatement
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_CB_PosttoGL {CompanyId},{UserId},{CBPettyCashHd.PaymentId},{(short)E_CB.CBPattyCash}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.CB,
                                TransactionId = (short)E_CB.CBPattyCash,
                                DocumentId = CBPettyCashHd.PaymentId,
                                DocumentNo = CBPettyCashHd.PaymentNo,
                                TblName = "CBPettyCashHd",
                                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                                Remarks = CBPettyCashHd.Remarks,
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                //Update Edit Version
                                if (IsEdit)
                                    await _repository.UpsertExecuteScalarAsync($"update CBPettyCashHd set EditVersion=EditVersion+1 where PaymentId={CBPettyCashHd.PaymentId}; Update CBPettyCashDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.CBPettyCashHd where PaymentId={CBPettyCashHd.PaymentId}) where PaymentId={CBPettyCashHd.PaymentId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_CB_PosttoGL {CompanyId},{UserId},{CBPettyCashHd.PaymentId},{(short)E_CB.CBPattyCash}");

                                TScope.Complete();
                                return new SqlResponse { Result = CBPettyCashHd.PaymentId, Message = "Save Successfully" };
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
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBPattyCash,
                    DocumentId = CBPettyCashHd.PaymentId,
                    DocumentNo = CBPettyCashHd.PaymentNo,
                    TblName = "CBPettyCashHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponse> DeleteCBPettyCashAsync(short CompanyId, long PaymentId, string CanacelRemarks, short UserId)
        {
            string PaymentNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Get Invoice Number
                    PaymentNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT PaymentNo FROM dbo.CBPettyCashHd WHERE PaymentId={PaymentId}");

                    if (PaymentId > 0)
                    {
                        //Update IsCancle=1,Cancelby=userid,Canceldate=now,CancelRemarks=CancelRemarks
                        var CBPettyCashToRemove = _context.CBPettyCashHd.Where(b => b.PaymentId == PaymentId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (CBPettyCashToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_CB_DeleteStatement {CompanyId},{UserId},{PaymentId},{(short)E_CB.CBPattyCash}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.CB,
                                TransactionId = (short)E_CB.CBPattyCash,
                                DocumentId = PaymentId,
                                DocumentNo = PaymentNo,
                                TblName = "CBPettyCashHd",
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
                    TransactionId = (short)E_CB.CBPattyCash,
                    DocumentId = PaymentId,
                    DocumentNo = PaymentNo,
                    TblName = "CBPettyCashHd",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CBPettyCashViewModel>> GetHistoryCBPettyCashByIdAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<CBPettyCashViewModel>($"SELECT Invhd.EditVersion,Invhd.CompanyId,Invhd.PaymentId,Invhd.PaymentNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.PaymentTypeId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,Invhd.ChequeNo,Invhd.ChequeDate,Invhd.BankChgAmt,Invhd.BankChgLocalAmt,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.Remarks,Invhd.ModuleFrom,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.CBPettyCashHd_Ver Invhd INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId LEFT JOIN dbo.M_PaymentType M_Pay ON M_Pay.PaymentTypeId = Invhd.PaymentTypeId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.PaymentId={PaymentId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.CB,
                    TransactionId = (short)E_CB.CBPattyCash,
                    DocumentId = PaymentId,
                    DocumentNo = PaymentNo,
                    TblName = "CBPettyCashHd",
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