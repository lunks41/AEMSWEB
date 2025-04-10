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
    public sealed class APCreditNoteService : IAPCreditNoteService
    {
        private readonly IRepository<ApCreditNoteHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public APCreditNoteService(IRepository<ApCreditNoteHd> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<APCreditNoteViewModelCount> GetAPCreditNoteListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId)
        {
            APCreditNoteViewModelCount countViewModel = new APCreditNoteViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.ApCreditNoteHd Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.CreditNoteNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND Invhd.CompanyId= {CompanyId}");
                var result = await _repository.GetQueryAsync<APCreditNoteViewModel>($"SELECT Invhd.CompanyId,Invhd.CreditNoteId,Invhd.CreditNoteNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.InvoiceId,Invhd.InvoiceNo,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.PurchaseOrderId,Invhd.PurchaseOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,M_Cun.CountryCode,M_Cun.CountryName,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.CustomerName,Invhd.SuppCreditNoteNo,Invhd.ArCreditNoteId,Invhd.ArCreditNoteNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.ApCreditNoteHd Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.CreditNoteNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') AND Invhd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId} ORDER BY Invhd.AccountDate Desc,Invhd.CreditNoteNo Desc OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<APCreditNoteViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.CreditNote,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "ApCreditNoteHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<APCreditNoteViewModel> GetAPCreditNoteByIdAsync(short CompanyId, long CreditNoteId, string CreditNoteNo, short UserId)
        {
            APCreditNoteViewModel aRCreditNoteViewModel = new APCreditNoteViewModel();
            try
            {
                aRCreditNoteViewModel = await _repository.GetQuerySingleOrDefaultAsync<APCreditNoteViewModel>($"SELECT Invhd.CompanyId,Invhd.CreditNoteId,Invhd.CreditNoteNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.InvoiceId,Invhd.InvoiceNo,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.PurchaseOrderId,Invhd.PurchaseOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,M_Cun.CountryCode,M_Cun.CountryName,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.CustomerName,Invhd.SuppCreditNoteNo,Invhd.ArCreditNoteId,Invhd.ArCreditNoteNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Invhd.EditVersion FROM dbo.ApCreditNoteHd Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.CreditNoteId={CreditNoteId} OR {CreditNoteId}=0) AND (Invhd.CreditNoteNo='{CreditNoteNo}' OR '{CreditNoteNo}'='{string.Empty}')");

                if (aRCreditNoteViewModel == null)
                    return aRCreditNoteViewModel;

                var result = await _repository.GetQueryAsync<APCreditNoteDtViewModel>($"SELECT Invdt.CreditNoteId,Invdt.CreditNoteNo,Invdt.ItemNo,Invdt.SeqNo,Invdt.DocItemNo,Invdt.ProductId,M_Pro.ProductCode,M_Pro.ProductName,Invdt.GLId,M_chra.GLCode,M_chra.GLName,Invdt.QTY,Invdt.BillQTY,Invdt.UomId,M_um.UomCode,M_um.UomName,Invdt.UnitPrice,Invdt.TotAmt,Invdt.TotLocalAmt,Invdt.TotCtyAmt,Invdt.Remarks,Invdt.GstId,M_Gs.GstCode,M_Gs.GstName,Invdt.GstPercentage,Invdt.GstAmt,Invdt.GstLocalAmt,Invdt.GstCtyAmt,Invdt.DeliveryDate,Invdt.DepartmentId,M_Dep.DepartmentCode,M_Dep.DepartmentName,Invdt.EmployeeId,M_Emp.EmployeeCode,M_Emp.EmployeeName,Invdt.PortId,M_Po.PortCode,M_Po.PortName,Invdt.VesselId,M_Vel.VesselCode,M_Vel.VesselName,Invdt.BargeId,M_Brg.BargeCode,M_Brg.BargeName,Invdt.VoyageId,M_Vo.VoyageNo,M_Vo.ReferenceNo as VoyageReferenceNo,Invdt.OperationId,Invdt.OperationNo,Invdt.OPRefNo,Invdt.PurchaseOrderId,Invdt.PurchaseOrderNo,Invdt.SupplyDate,Invdt.CustomerName,Invdt.CustCreditNoteNo,Invdt.ArCreditNoteId,Invdt.ArCreditNoteNo,Invdt.EditVersion FROM dbo.ApCreditNoteDt Invdt LEFT JOIN dbo.M_Uom M_um ON M_um.UomId = Invdt.UomId LEFT JOIN dbo.M_ChartOfAccount M_chra ON M_chra.GLId = Invdt.GLId LEFT JOIN dbo.M_Product M_Pro ON M_Pro.ProductId = Invdt.ProductId LEFT JOIN dbo.M_Gst M_Gs ON M_Gs.GstId = Invdt.GstId LEFT JOIN dbo.M_Department M_Dep ON M_Dep.DepartmentId = Invdt.DepartmentId LEFT JOIN dbo.M_Employee M_Emp ON M_Emp.EmployeeId = Invdt.EmployeeId LEFT JOIN dbo.M_Port M_Po ON M_Po.PortId = Invdt.PortId LEFT JOIN dbo.M_Vessel M_Vel ON M_Vel.VesselId = Invdt.VesselId LEFT JOIN dbo.M_Barge M_Brg ON M_Brg.BargeId = Invdt.BargeId LEFT JOIN dbo.M_Voyage M_Vo ON M_Vo.VoyageId = Invdt.VoyageId  WHERE Invdt.CreditNoteId={aRCreditNoteViewModel.CreditNoteId}");

                aRCreditNoteViewModel.data_details = result == null ? null : result.ToList();

                return aRCreditNoteViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.CreditNote,
                    DocumentId = CreditNoteId,
                    DocumentNo = CreditNoteNo,
                    TblName = "ApCreditNoteHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception($"An error occurred: {ex.Message}");
            }
        }

        public async Task<SqlResponce> SaveAPCreditNoteAsync(short CompanyId, ApCreditNoteHd arCreditNoteHd, List<ApCreditNoteDt> arCreditNoteDt, short UserId)
        {
            bool IsEdit = false;
            string accountDate = arCreditNoteHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (arCreditNoteHd.CreditNoteId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.ApCreditNoteHd WHERE IsCancel=0 And CompanyId={CompanyId} And CreditNoteId={arCreditNoteHd.CreditNoteId}");

                        if (dataExist.Count() == 0)
                            return new SqlResponce { Result = -1, Message = "CreditNote Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponceIds>($"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.AP},{(short)E_AP.CreditNote},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            arCreditNoteHd.CreditNoteId = documentIdNo.ToList()[0].DocumentId;
                            arCreditNoteHd.CreditNoteNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "CreditNote Number can't generate" };
                    }
                    else
                    {
                        //Insert the previous arCreditNote record to arCreditNote history table as well as editversion also.
                        await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AP_CreateHistoryRec {CompanyId},{UserId},{arCreditNoteHd.CreditNoteId},{(short)E_AP.CreditNote}");
                    }

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(arCreditNoteHd);
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
                        arCreditNoteHd.EditDate = null;
                        arCreditNoteHd.EditById = null;

                        var entityHead = _context.Add(arCreditNoteHd);

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
                            _context.ApCreditNoteDt.Where(x => x.CreditNoteId == arCreditNoteHd.CreditNoteId).ExecuteDelete();

                        foreach (var item in arCreditNoteDt)
                        {
                            item.CreditNoteId = arCreditNoteHd.CreditNoteId;
                            item.CreditNoteNo = arCreditNoteHd.CreditNoteNo;
                            _context.Add(item);
                        }

                        var SaveDetails = _context.SaveChanges();

                        #region Save AuditLog

                        if (SaveDetails > 0)
                        {
                            //Inserting the records into AP CreateStatement
                            await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AP_CreateStatement {CompanyId},{UserId},{arCreditNoteHd.CreditNoteId},{(short)E_AP.CreditNote}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AP,
                                TransactionId = (short)E_AP.CreditNote,
                                DocumentId = arCreditNoteHd.CreditNoteId,
                                DocumentNo = arCreditNoteHd.CreditNoteNo,
                                TblName = "ApCreditNoteHd",
                                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                                Remarks = arCreditNoteHd.Remarks,
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                //Update Edit Version
                                if (IsEdit)
                                    await _repository.UpsertExecuteScalarAsync($"update ApCreditNoteHd set EditVersion=EditVersion+1 where CreditNoteId={arCreditNoteHd.CreditNoteId}; Update ApCreditNoteDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.ApCreditNoteHd where CreditNoteId={arCreditNoteHd.CreditNoteId}) where CreditNoteId={arCreditNoteHd.CreditNoteId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AP_CreateStatement {CompanyId},{UserId},{arCreditNoteHd.CreditNoteId},{(short)E_AP.CreditNote}");

                                TScope.Complete();
                                return new SqlResponce { Result = arCreditNoteHd.CreditNoteId, Message = "Save Successfully" };
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
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.CreditNote,
                    DocumentId = arCreditNoteHd.CreditNoteId,
                    DocumentNo = arCreditNoteHd.CreditNoteNo,
                    TblName = "ApCreditNoteHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponce> DeleteAPCreditNoteAsync(short CompanyId, long CreditNoteId, string CanacelRemarks, short UserId)
        {
            string CreditNoteNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Get CreditNote Number
                    CreditNoteNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT CreditNoteNo FROM dbo.ApCreditNoteHd WHERE CreditNoteId={CreditNoteId}");

                    if (CreditNoteId > 0 && CreditNoteNo != null)
                    {
                        //Update IsCancle=1,Cancelby=userid,Canceldate=now,CancelRemarks=CancelRemarks
                        var APCreditNoteToRemove = _context.ApCreditNoteHd.Where(b => b.CreditNoteId == CreditNoteId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (APCreditNoteToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AP_DeleteStatement {CompanyId},{UserId},{CreditNoteId},{(short)E_AP.CreditNote}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AP,
                                TransactionId = (short)E_AP.CreditNote,
                                DocumentId = CreditNoteId,
                                DocumentNo = CreditNoteNo,
                                TblName = "ApCreditNoteHd",
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
                        return new SqlResponce { Result = -1, Message = "CreditNote Not exists" };
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
                    TransactionId = (short)E_AP.CreditNote,
                    DocumentId = CreditNoteId,
                    DocumentNo = CreditNoteNo,
                    TblName = "ApCreditNoteHd",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<APCreditNoteViewModel>> GetHistoryAPCreditNoteByIdAsync(short CompanyId, long CreditNoteId, string CreditNoteNo, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<APCreditNoteViewModel>($"SELECT Invhd.EditVersion, Invhd.CompanyId,Invhd.CreditNoteId,Invhd.CreditNoteNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.PurchaseOrderId,Invhd.PurchaseOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,M_Cun.CountryCode,M_Cun.CountryName,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.CustomerName,Invhd.SuppCreditNoteNo,Invhd.ArCreditNoteId,Invhd.ArCreditNoteNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.ApCreditNoteHd_Ver Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.CreditNoteId={CreditNoteId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.CreditNote,
                    DocumentId = CreditNoteId,
                    DocumentNo = CreditNoteNo,
                    TblName = "ApCreditNoteHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<APCreditNoteViewModel> GetHistoryAPCreditNoteByIdAsync_V1(short CompanyId, long CreditNoteId, string CreditNoteNo, short UserId)
        {
            APCreditNoteViewModel aRCreditNoteViewModel = new APCreditNoteViewModel();
            try
            {
                aRCreditNoteViewModel = await _repository.GetQuerySingleOrDefaultAsync<APCreditNoteViewModel>($"SELECT Invhd.CompanyId,Invhd.CreditNoteId,Invhd.CreditNoteNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.PurchaseOrderId,Invhd.PurchaseOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,M_Cun.CountryCode,M_Cun.CountryName,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.CustomerName,Invhd.SuppCreditNoteNo,Invhd.ArCreditNoteId,Invhd.ArCreditNoteNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.ApCreditNoteHd_Ver Invhd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Invhd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.CreditNoteId={CreditNoteId} OR {CreditNoteId}=0) AND (Invhd.CreditNoteNo='{CreditNoteNo}' OR '{CreditNoteNo}'='{string.Empty}')");

                if (aRCreditNoteViewModel == null)
                    return aRCreditNoteViewModel;

                var result = await _repository.GetQueryAsync<APCreditNoteDtViewModel>($"SELECT Invdt.CreditNoteId,Invdt.CreditNoteNo,Invdt.ItemNo,Invdt.SeqNo,Invdt.DocItemNo,Invdt.ProductId,M_Pro.ProductCode,M_Pro.ProductName,Invdt.GLId,M_chra.GLCode,M_chra.GLName,Invdt.QTY,Invdt.BillQTY,Invdt.UomId,M_um.UomCode,M_um.UomName,Invdt.UnitPrice,Invdt.TotAmt,Invdt.TotLocalAmt,Invdt.TotCtyAmt,Invdt.Remarks,Invdt.GstId,M_Gs.GstCode,M_Gs.GstName,Invdt.GstPercentage,Invdt.GstAmt,Invdt.GstLocalAmt,Invdt.GstCtyAmt,Invdt.DeliveryDate,Invdt.DepartmentId,M_Dep.DepartmentCode,M_Dep.DepartmentName,Invdt.EmployeeId,M_Emp.EmployeeCode,M_Emp.EmployeeName,Invdt.PortId,M_Po.PortCode,M_Po.PortName,Invdt.VesselId,M_Vel.VesselCode,M_Vel.VesselName,Invdt.BargeId,M_Brg.BargeCode,M_Brg.BargeName,Invdt.VoyageId,M_Vo.VoyageNo,M_Vo.ReferenceNo as VoyageReferenceNo,Invdt.OperationId,Invdt.OperationNo,Invdt.OPRefNo,Invdt.PurchaseOrderId,Invdt.PurchaseOrderNo,Invdt.SupplyDate,Invdt.CustomerName,Invdt.CustCreditNoteNo,Invdt.ArCreditNoteId,Invdt.ArCreditNoteNo,Invdt.EditVersion FROM dbo.ApCreditNoteDt_Ver Invdt LEFT JOIN dbo.M_Uom M_um ON M_um.UomId = Invdt.UomId LEFT JOIN dbo.M_ChartOfAccount M_chra ON M_chra.GLId = Invdt.GLId LEFT JOIN dbo.M_Product M_Pro ON M_Pro.ProductId = Invdt.ProductId LEFT JOIN dbo.M_Gst M_Gs ON M_Gs.GstId = Invdt.GstId LEFT JOIN dbo.M_Department M_Dep ON M_Dep.DepartmentId = Invdt.DepartmentId LEFT JOIN dbo.M_Employee M_Emp ON M_Emp.EmployeeId = Invdt.EmployeeId LEFT JOIN dbo.M_Port M_Po ON M_Po.PortId = Invdt.PortId LEFT JOIN dbo.M_Vessel M_Vel ON M_Vel.VesselId = Invdt.VesselId LEFT JOIN dbo.M_Barge M_Brg ON M_Brg.BargeId = Invdt.BargeId LEFT JOIN dbo.M_Voyage M_Vo ON M_Vo.VoyageId = Invdt.VoyageId  WHERE Invdt.CreditNoteId={aRCreditNoteViewModel.CreditNoteId}");

                aRCreditNoteViewModel.data_details = result == null ? null : result.ToList();

                return aRCreditNoteViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.CreditNote,
                    DocumentId = CreditNoteId,
                    DocumentNo = CreditNoteNo,
                    TblName = "ApCreditNoteHd",
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