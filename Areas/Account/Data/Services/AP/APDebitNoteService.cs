using AEMSWEB.Areas.Account.Data.IServices;
using AEMSWEB.Areas.Account.Data.IServices.AP;
using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Data;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Account.Data.Services.AP
{
    public sealed class APDebitNoteService : IAPDebitNoteService
    {
        private readonly IRepository<ApDebitNoteHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public APDebitNoteService(IRepository<ApDebitNoteHd> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<APDebitNoteViewModelCount> GetAPDebitNoteListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, string fromDate, string toDate, short UserId)
        {
            APDebitNoteViewModelCount countViewModel = new APDebitNoteViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.ApDebitNoteHd Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.DebitNoteNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND Invhd.CompanyId= {CompanyId}");
                var result = await _repository.GetQueryAsync<APDebitNoteViewModel>($"SELECT Invhd.CompanyId,Invhd.DebitNoteId,Invhd.DebitNoteNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.InvoiceId,Invhd.InvoiceNo,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.PurchaseOrderId,Invhd.PurchaseOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,M_Cun.CountryCode,M_Cun.CountryName,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.CustomerName,Invhd.SuppDebitNoteNo,Invhd.ArDebitNoteId,Invhd.ArDebitNoteNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.ApDebitNoteHd Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.DebitNoteNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND Invhd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId} ORDER BY Invhd.AccountDate Desc,Invhd.DebitNoteNo Desc OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.DebitNote,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "ApDebitNoteHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<APDebitNoteViewModel> GetAPDebitNoteByIdAsync(short CompanyId, long DebitNoteId, string DebitNoteNo, short UserId)
        {
            APDebitNoteViewModel aRDebitNoteViewModel = new APDebitNoteViewModel();
            try
            {
                aRDebitNoteViewModel = await _repository.GetQuerySingleOrDefaultAsync<APDebitNoteViewModel>($"SELECT Invhd.CompanyId,Invhd.DebitNoteId,Invhd.DebitNoteNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.InvoiceId,Invhd.InvoiceNo,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.PurchaseOrderId,Invhd.PurchaseOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,M_Cun.CountryCode,M_Cun.CountryName,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.CustomerName,Invhd.SuppDebitNoteNo,Invhd.ArDebitNoteId,Invhd.ArDebitNoteNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.ApDebitNoteHd Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.DebitNoteId={DebitNoteId} OR {DebitNoteId}=0) AND (Invhd.DebitNoteNo='{DebitNoteNo}' OR '{DebitNoteNo}'='{string.Empty}')");

                if (aRDebitNoteViewModel == null)
                    return aRDebitNoteViewModel;

                var result = await _repository.GetQueryAsync<APDebitNoteDtViewModel>($"SELECT Invdt.DebitNoteId,Invdt.DebitNoteNo,Invdt.ItemNo,Invdt.SeqNo,Invdt.DocItemNo,Invdt.ProductId,M_Pro.ProductCode,M_Pro.ProductName,Invdt.GLId,M_chra.GLCode,M_chra.GLName,Invdt.QTY,Invdt.BillQTY,Invdt.UomId,M_um.UomCode,M_um.UomName,Invdt.UnitPrice,Invdt.TotAmt,Invdt.TotLocalAmt,Invdt.TotCtyAmt,Invdt.Remarks,Invdt.GstId,M_Gs.GstCode,M_Gs.GstName,Invdt.GstPercentage,Invdt.GstAmt,Invdt.GstLocalAmt,Invdt.GstCtyAmt,Invdt.DeliveryDate,Invdt.DepartmentId,M_Dep.DepartmentCode,M_Dep.DepartmentName,Invdt.EmployeeId,M_Emp.EmployeeCode,M_Emp.EmployeeName,Invdt.PortId,M_Po.PortCode,M_Po.PortName,Invdt.VesselId,M_Vel.VesselCode,M_Vel.VesselName,Invdt.BargeId,M_Brg.BargeCode,M_Brg.BargeName,Invdt.VoyageId,M_Vo.VoyageNo,M_Vo.ReferenceNo as VoyageReferenceNo,Invdt.OperationId,Invdt.OperationNo,Invdt.OPRefNo,Invdt.PurchaseOrderId,Invdt.PurchaseOrderNo,Invdt.SupplyDate,Invdt.CustomerName,Invdt.CustDebitNoteNo,Invdt.ArDebitNoteId,Invdt.ArDebitNoteNo,Invdt.EditVersion FROM dbo.ApDebitNoteDt Invdt LEFT JOIN dbo.M_Uom M_um ON M_um.UomId = Invdt.UomId LEFT JOIN dbo.M_ChartOfAccount M_chra ON M_chra.GLId = Invdt.GLId LEFT JOIN dbo.M_Product M_Pro ON M_Pro.ProductId = Invdt.ProductId LEFT JOIN dbo.M_Gst M_Gs ON M_Gs.GstId = Invdt.GstId LEFT JOIN dbo.M_Department M_Dep ON M_Dep.DepartmentId = Invdt.DepartmentId LEFT JOIN dbo.M_Employee M_Emp ON M_Emp.EmployeeId = Invdt.EmployeeId LEFT JOIN dbo.M_Port M_Po ON M_Po.PortId = Invdt.PortId LEFT JOIN dbo.M_Vessel M_Vel ON M_Vel.VesselId = Invdt.VesselId LEFT JOIN dbo.M_Barge M_Brg ON M_Brg.BargeId = Invdt.BargeId LEFT JOIN dbo.M_Voyage M_Vo ON M_Vo.VoyageId = Invdt.VoyageId  WHERE Invdt.DebitNoteId={aRDebitNoteViewModel.DebitNoteId}");

                aRDebitNoteViewModel.data_details = result == null ? null : result.ToList();

                return aRDebitNoteViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.DebitNote,
                    DocumentId = DebitNoteId,
                    DocumentNo = DebitNoteNo,
                    TblName = "ApDebitNoteHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception($"An error occurred: {ex.Message}");
            }
        }

        public async Task<SqlResponse> SaveAPDebitNoteAsync(short CompanyId, ApDebitNoteHd arDebitNoteHd, List<ApDebitNoteDt> arDebitNoteDt, short UserId)
        {
            bool IsEdit = false;
            string accountDate = arDebitNoteHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (arDebitNoteHd.DebitNoteId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.ApDebitNoteHd WHERE IsCancel=0 And CompanyId={CompanyId} And DebitNoteId={arDebitNoteHd.DebitNoteId}");

                        if (dataExist.Count() == 0)
                            return new SqlResponse { Result = -1, Message = "DebitNote Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponseIds>($"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.AP},{(short)E_AP.DebitNote},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            arDebitNoteHd.DebitNoteId = documentIdNo.ToList()[0].DocumentId;
                            arDebitNoteHd.DebitNoteNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "DebitNote Number can't generate" };
                    }
                    else
                    {
                        //Insert the previous arDebitNote record to arDebitNote history table as well as editversion also.
                        await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AP_CreateHistoryRec {CompanyId},{UserId},{arDebitNoteHd.DebitNoteId},{(short)E_AP.DebitNote}");
                    }

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(arDebitNoteHd);
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
                        arDebitNoteHd.EditDate = null;
                        arDebitNoteHd.EditById = null;

                        var entityHead = _context.Add(arDebitNoteHd);

                        entityHead.Property(b => b.IsCancel).IsModified = false;
                        entityHead.Property(b => b.CancelById).IsModified = false;
                        entityHead.Property(b => b.CancelDate).IsModified = false;
                        entityHead.Property(b => b.CancelRemarks).IsModified = false;
                    }

                    var SaveHeader = _context.SaveChanges();

                    //Saving Details
                    if (SaveHeader > 0)
                    {
                        if (IsEdit)
                            _context.ApDebitNoteDt.Where(x => x.DebitNoteId == arDebitNoteHd.DebitNoteId).ExecuteDelete();

                        foreach (var item in arDebitNoteDt)
                        {
                            item.DebitNoteId = arDebitNoteHd.DebitNoteId;
                            item.DebitNoteNo = arDebitNoteHd.DebitNoteNo;
                            _context.Add(item);
                        }

                        var SaveDetails = _context.SaveChanges();

                        #region Save AuditLog

                        if (SaveDetails > 0)
                        {
                            //Inserting the records into AP CreateStatement
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AP_CreateStatement {CompanyId},{UserId},{arDebitNoteHd.DebitNoteId},{(short)E_AP.DebitNote}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AP,
                                TransactionId = (short)E_AP.DebitNote,
                                DocumentId = arDebitNoteHd.DebitNoteId,
                                DocumentNo = arDebitNoteHd.DebitNoteNo,
                                TblName = "ApDebitNoteHd",
                                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                                Remarks = arDebitNoteHd.Remarks,
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                //Update Edit Version
                                if (IsEdit)
                                    await _repository.UpsertExecuteScalarAsync($"update ApDebitNoteHd set EditVersion=EditVersion+1 where DebitNoteId={arDebitNoteHd.DebitNoteId}; Update ApDebitNoteDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.ApDebitNoteHd where DebitNoteId={arDebitNoteHd.DebitNoteId}) where DebitNoteId={arDebitNoteHd.DebitNoteId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AP_CreateStatement {CompanyId},{UserId},{arDebitNoteHd.DebitNoteId},{(short)E_AP.DebitNote}");

                                TScope.Complete();
                                return new SqlResponse { Result = arDebitNoteHd.DebitNoteId, Message = "Save Successfully" };
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
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.DebitNote,
                    DocumentId = arDebitNoteHd.DebitNoteId,
                    DocumentNo = arDebitNoteHd.DebitNoteNo,
                    TblName = "ApDebitNoteHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponse> DeleteAPDebitNoteAsync(short CompanyId, long DebitNoteId, string CanacelRemarks, short UserId)
        {
            string DebitNoteNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Get DebitNote Number
                    DebitNoteNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT DebitNoteNo FROM dbo.ApDebitNoteHd WHERE DebitNoteId={DebitNoteId}");

                    if (DebitNoteId > 0 && DebitNoteNo != null)
                    {
                        //Update IsCancle=1,Cancelby=userid,Canceldate=now,CancelRemarks=CancelRemarks
                        var APDebitNoteToRemove = _context.ApDebitNoteHd.Where(b => b.DebitNoteId == DebitNoteId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (APDebitNoteToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AP_DeleteStatement {CompanyId},{UserId},{DebitNoteId},{(short)E_AP.DebitNote}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AP,
                                TransactionId = (short)E_AP.DebitNote,
                                DocumentId = DebitNoteId,
                                DocumentNo = DebitNoteNo,
                                TblName = "ApDebitNoteHd",
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
                        return new SqlResponse { Result = -1, Message = "DebitNote Not exists" };
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
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.DebitNote,
                    DocumentId = DebitNoteId,
                    DocumentNo = DebitNoteNo,
                    TblName = "ApDebitNoteHd",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<APDebitNoteViewModel>> GetHistoryAPDebitNoteByIdAsync(short CompanyId, long DebitNoteId, string DebitNoteNo, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<APDebitNoteViewModel>($"SELECT Invhd.EditVersion, Invhd.CompanyId,Invhd.DebitNoteId,Invhd.DebitNoteNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.PurchaseOrderId,Invhd.PurchaseOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,M_Cun.CountryCode,M_Cun.CountryName,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.CustomerName,Invhd.SuppDebitNoteNo,Invhd.ArDebitNoteId,Invhd.ArDebitNoteNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.ApDebitNoteHd_Ver Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.DebitNoteId={DebitNoteId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.DebitNote,
                    DocumentId = DebitNoteId,
                    DocumentNo = DebitNoteNo,
                    TblName = "ApDebitNoteHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<APDebitNoteViewModel> GetHistoryAPDebitNoteByIdAsync_V1(short CompanyId, long DebitNoteId, string DebitNoteNo, short UserId)
        {
            APDebitNoteViewModel aRDebitNoteViewModel = new APDebitNoteViewModel();
            try
            {
                aRDebitNoteViewModel = await _repository.GetQuerySingleOrDefaultAsync<APDebitNoteViewModel>($"SELECT Invhd.CompanyId,Invhd.DebitNoteId,Invhd.DebitNoteNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.PurchaseOrderId,Invhd.PurchaseOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,M_Cun.CountryCode,M_Cun.CountryName,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.CustomerName,Invhd.SuppDebitNoteNo,Invhd.ArDebitNoteId,Invhd.ArDebitNoteNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.ApDebitNoteHd_Ver Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.DebitNoteId={DebitNoteId} OR {DebitNoteId}=0) AND (Invhd.DebitNoteNo='{DebitNoteNo}' OR '{DebitNoteNo}'='{string.Empty}')");

                if (aRDebitNoteViewModel == null)
                    return aRDebitNoteViewModel;

                var result = await _repository.GetQueryAsync<APDebitNoteDtViewModel>($"SELECT Invdt.DebitNoteId,Invdt.DebitNoteNo,Invdt.ItemNo,Invdt.SeqNo,Invdt.DocItemNo,Invdt.ProductId,M_Pro.ProductCode,M_Pro.ProductName,Invdt.GLId,M_chra.GLCode,M_chra.GLName,Invdt.QTY,Invdt.BillQTY,Invdt.UomId,M_um.UomCode,M_um.UomName,Invdt.UnitPrice,Invdt.TotAmt,Invdt.TotLocalAmt,Invdt.TotCtyAmt,Invdt.Remarks,Invdt.GstId,M_Gs.GstCode,M_Gs.GstName,Invdt.GstPercentage,Invdt.GstAmt,Invdt.GstLocalAmt,Invdt.GstCtyAmt,Invdt.DeliveryDate,Invdt.DepartmentId,M_Dep.DepartmentCode,M_Dep.DepartmentName,Invdt.EmployeeId,M_Emp.EmployeeCode,M_Emp.EmployeeName,Invdt.PortId,M_Po.PortCode,M_Po.PortName,Invdt.VesselId,M_Vel.VesselCode,M_Vel.VesselName,Invdt.BargeId,M_Brg.BargeCode,M_Brg.BargeName,Invdt.VoyageId,M_Vo.VoyageNo,M_Vo.ReferenceNo as VoyageReferenceNo,Invdt.OperationId,Invdt.OperationNo,Invdt.OPRefNo,Invdt.PurchaseOrderId,Invdt.PurchaseOrderNo,Invdt.SupplyDate,Invdt.CustomerName,Invdt.CustDebitNoteNo,Invdt.ArDebitNoteId,Invdt.ArDebitNoteNo,Invdt.EditVersion FROM dbo.ApDebitNoteDt_Ver Invdt LEFT JOIN dbo.M_Uom M_um ON M_um.UomId = Invdt.UomId LEFT JOIN dbo.M_ChartOfAccount M_chra ON M_chra.GLId = Invdt.GLId LEFT JOIN dbo.M_Product M_Pro ON M_Pro.ProductId = Invdt.ProductId LEFT JOIN dbo.M_Gst M_Gs ON M_Gs.GstId = Invdt.GstId LEFT JOIN dbo.M_Department M_Dep ON M_Dep.DepartmentId = Invdt.DepartmentId LEFT JOIN dbo.M_Employee M_Emp ON M_Emp.EmployeeId = Invdt.EmployeeId LEFT JOIN dbo.M_Port M_Po ON M_Po.PortId = Invdt.PortId LEFT JOIN dbo.M_Vessel M_Vel ON M_Vel.VesselId = Invdt.VesselId LEFT JOIN dbo.M_Barge M_Brg ON M_Brg.BargeId = Invdt.BargeId LEFT JOIN dbo.M_Voyage M_Vo ON M_Vo.VoyageId = Invdt.VoyageId  WHERE Invdt.DebitNoteId={aRDebitNoteViewModel.DebitNoteId}");

                aRDebitNoteViewModel.data_details = result == null ? null : result.ToList();

                return aRDebitNoteViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.DebitNote,
                    DocumentId = DebitNoteId,
                    DocumentNo = DebitNoteNo,
                    TblName = "ApDebitNoteHd",
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