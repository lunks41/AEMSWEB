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
    public sealed class APDocSetOffService : IAPDocSetOffService
    {
        private readonly IRepository<ApDocSetOffHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public APDocSetOffService(IRepository<ApDocSetOffHd> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<APDocSetOffViewModelCount> GetAPDocSetOffListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId)
        {
            APDocSetOffViewModelCount countViewModel = new APDocSetOffViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.ApDocSetOffHd Dochd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Dochd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Dochd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Dochd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Dochd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Dochd.CancelById WHERE (Dochd.SetoffNo LIKE '%{searchString}%' OR Dochd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.SupplierCode LIKE '%{searchString}%' OR M_Cus.SupplierName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND TrnDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<APDocSetOffViewModel>($"SELECT Dochd.CompanyId,Dochd.SetoffId,Dochd.SetoffNo,Dochd.ReferenceNo,Dochd.TrnDate,Dochd.AccountDate,Dochd.SupplierId,M_Cus.SupplierCode,M_Cus.SupplierName,Dochd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Dochd.ExhRate,Dochd.BalanceAmt,Dochd.AllocAmt,Dochd.Remarks,Dochd.UnAllocAmt,Dochd.ExhGainLoss,Dochd.ModuleFrom,Dochd.CreateById,Dochd.CreateDate,Dochd.EditById,Dochd.EditDate,Dochd.CancelById,Dochd.CancelDate,Dochd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Dochd.EditVersion FROM dbo.ApDocSetOffHd Dochd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Dochd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Dochd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Dochd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Dochd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Dochd.CancelById WHERE (Dochd.SetoffNo LIKE '%{searchString}%' OR Dochd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.SupplierCode LIKE '%{searchString}%' OR M_Cus.SupplierName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND Dochd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId} ORDER BY Dochd.SetoffNo,Dochd.AccountDate OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<APDocSetOffViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AR.DocSetoff,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "ApDocSetOffHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<APDocSetOffViewModel> GetAPDocSetOffByIdAsync(short CompanyId, long SetoffId, string SetoffNo, short UserId)
        {
            APDocSetOffViewModel APDocSetOffViewModel = new APDocSetOffViewModel();
            try
            {
                APDocSetOffViewModel = await _repository.GetQuerySingleOrDefaultAsync<APDocSetOffViewModel>($"SELECT Dochd.CompanyId,Dochd.SetoffId,Dochd.SetoffNo,Dochd.ReferenceNo,Dochd.TrnDate,Dochd.AccountDate,Dochd.SupplierId,M_Cus.SupplierCode,M_Cus.SupplierName,Dochd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Dochd.ExhRate,Dochd.BalanceAmt,Dochd.AllocAmt,Dochd.UnAllocAmt,Dochd.ExhGainLoss,Dochd.Remarks,Dochd.ModuleFrom,Dochd.CreateById,Dochd.CreateDate,Dochd.EditById,Dochd.EditDate,Dochd.CancelById,Dochd.CancelDate,Dochd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Dochd.EditVersion FROM dbo.ApDocSetOffHd Dochd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Dochd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Dochd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Dochd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Dochd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Dochd.CancelById  WHERE (Dochd.SetoffId={SetoffId} OR {SetoffId}=0) AND (Dochd.SetoffNo='{SetoffNo}' OR '{SetoffNo}'='{string.Empty}')");

                if (APDocSetOffViewModel == null)
                    return APDocSetOffViewModel;

                var result = await _repository.GetQueryAsync<APDocSetOffDtViewModel>($"SELECT Docdt.CompanyId,Docdt.SetoffId,Docdt.SetoffNo,Docdt.ItemNo,Docdt.TransactionId,Docdt.DocumentId,Docdt.DocumentNo,Docdt.ReferenceNo,Docdt.DocCurrencyId,Docdt.DocExhRate,Docdt.DocAccountDate,Docdt.DocDueDate,Docdt.DocTotAmt,Docdt.DocTotLocalAmt,Docdt.DocBalAmt,Docdt.DocBalLocalAmt,Docdt.AllocAmt,Docdt.AllocLocalAmt,Docdt.DocAllocAmt,Docdt.DocAllocLocalAmt,Docdt.CentDiff,Docdt.ExhGainLoss FROM dbo.ApDocSetOffDt Docdt  WHERE Docdt.SetoffId={APDocSetOffViewModel.SetoffId}");

                APDocSetOffViewModel.data_details = result == null ? null : result.ToList();

                return APDocSetOffViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AR.DocSetoff,
                    DocumentId = SetoffId,
                    DocumentNo = SetoffNo,
                    TblName = "ApDocSetOffHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveAPDocSetOffAsync(short CompanyId, ApDocSetOffHd arDocSetOffHd, List<ApDocSetOffDt> arDocSetOffDts, short UserId)
        {
            bool IsEdit = false;
            string accountDate = arDocSetOffHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //var IsPeriodClosed = await _repository.GetQuerySingleOrDefaultAsync<bool>( $"SELECT dbo.CheckPeriodClosed({CompanyId},{(short)E_Modules.AP},'{accountDate}') as IsExist");

                    //if (!IsPeriodClosed)
                    //{
                    if (arDocSetOffHd.SetoffId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.ApDocSetOffHd WHERE IsCancel=0 And CompanyId={CompanyId} And SetoffId={arDocSetOffHd.SetoffId}");

                        if (dataExist.Count() == 0)
                            return new SqlResponce { Result = -1, Message = "DocSetOff Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponceIds>($"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.AP},{(short)E_AR.DocSetoff},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            arDocSetOffHd.SetoffId = documentIdNo.ToList()[0].DocumentId;
                            arDocSetOffHd.SetoffNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        //Insert the previous APDocSetOff record to APDocSetOff history table as well as editversion also.
                        await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_CreateHistoryRec {CompanyId},{UserId},{arDocSetOffHd.SetoffId},{(short)E_AR.DocSetoff}");
                    }

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(arDocSetOffHd);
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
                        arDocSetOffHd.EditDate = null;
                        arDocSetOffHd.EditById = null;
                        arDocSetOffHd.CancelDate = null;
                        arDocSetOffHd.CancelById = null;

                        var entityHead = _context.Add(arDocSetOffHd);
                    }

                    var SaveHeader = _context.SaveChanges();

                    //Saving Details
                    if (SaveHeader > 0)
                    {
                        if (IsEdit)
                            _context.ApDocSetOffDt.Where(x => x.SetoffId == arDocSetOffHd.SetoffId).ExecuteDelete();

                        foreach (var item in arDocSetOffDts)
                        {
                            item.SetoffId = arDocSetOffHd.SetoffId;
                            item.SetoffNo = arDocSetOffHd.SetoffNo;
                            _context.Add(item);
                        }
                        var SaveDetails = _context.SaveChanges();

                        #region Save AuditLog

                        if (SaveDetails > 0)
                        {
                            //Inserting the records into AR CreateStatement
                            await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arDocSetOffHd.SetoffId},{(short)E_AR.DocSetoff}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AP,
                                TransactionId = (short)E_AR.DocSetoff,
                                DocumentId = arDocSetOffHd.SetoffId,
                                DocumentNo = arDocSetOffHd.SetoffNo,
                                TblName = "ApDocSetOffHd",
                                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                                Remarks = arDocSetOffHd.Remarks,
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                //Update Edit Version
                                if (IsEdit)
                                    await _repository.UpsertExecuteScalarAsync($"update ApDocSetOffHd set EditVersion=EditVersion+1 where SetoffId={arDocSetOffHd.SetoffId}; Update ApDocSetOffDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.ApDocSetOffHd where SetoffId={arDocSetOffHd.SetoffId}) where SetoffId={arDocSetOffHd.SetoffId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arDocSetOffHd.SetoffId},{(short)E_AR.DocSetoff}");

                                TScope.Complete();
                                return new SqlResponce { Result = arDocSetOffHd.SetoffId, Message = "Save Successfully" };
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
                    TransactionId = (short)E_AR.DocSetoff,
                    DocumentId = arDocSetOffHd.SetoffId,
                    DocumentNo = arDocSetOffHd.SetoffNo,
                    TblName = "ApDocSetOffHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponce> DeleteAPDocSetOffAsync(short CompanyId, long SetoffId, string SetoffNo, string CanacelRemarks, short UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (SetoffId > 0)
                    {
                        var APDocSetOffToRemove = _context.ApDocSetOffHd.Where(b => b.SetoffId == SetoffId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (APDocSetOffToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponceIds>($"exec FIN_AR_DeleteStatement {CompanyId},{UserId},{SetoffId},{(short)E_AR.DocSetoff}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AP,
                                TransactionId = (short)E_AR.DocSetoff,
                                DocumentId = SetoffId,
                                DocumentNo = SetoffNo,
                                TblName = "ApDocSetOffHd",
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
                        return new SqlResponce { Result = -1, Message = "DocSetOff Not exists" };
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
                    TransactionId = (short)E_AR.DocSetoff,
                    DocumentId = SetoffId,
                    DocumentNo = SetoffNo,
                    TblName = "ApDocSetOffHd",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<APDocSetOffViewModel>> GetHistoryAPDocSetOffByIdAsync(short CompanyId, long SetoffId, string SetoffNo, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<APDocSetOffViewModel>($"SELECT Dochd.CompanyId,Dochd.SetoffId,Dochd.SetoffNo,Dochd.ReferenceNo,Dochd.TrnDate,Dochd.AccountDate,Dochd.SupplierId,M_Cus.SupplierCode,M_Cus.SupplierName,Dochd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Dochd.ExhRate,Dochd.BalanceAmt,Dochd.AllocAmt,Dochd.UnAllocAmt,Dochd.ExhGainLoss,Dochd.ModuleFrom,Dochd.CreateById,Dochd.CreateDate,Dochd.EditById,Dochd.EditDate,Dochd.CancelById,Dochd.CancelDate,Dochd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Dochd.EditVersion FROM dbo.ApDocSetOffHd_Ver Dochd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = Dochd.SupplierId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Dochd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Dochd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Dochd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Dochd.CancelById  WHERE (Dochd.SetoffId={SetoffId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AR.DocSetoff,
                    DocumentId = SetoffId,
                    DocumentNo = SetoffNo,
                    TblName = "ApDocSetOffHd",
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