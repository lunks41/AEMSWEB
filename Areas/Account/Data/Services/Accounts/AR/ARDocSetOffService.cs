using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Data;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.IServices.Accounts;
using AEMSWEB.IServices.Accounts.AR;
using AEMSWEB.Models;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Services.Accounts.AR
{
    public sealed class ARDocSetOffService : IARDocSetOffService
    {
        private readonly IRepository<ArDocSetOffHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public ARDocSetOffService(IRepository<ArDocSetOffHd> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<ARDocSetOffViewModelCount> GetARDocSetOffListAsync(Int16 CompanyId, short pageSize, short pageNumber, string searchString, string fromDate, string toDate, Int16 UserId)
        {
            ARDocSetOffViewModelCount countViewModel = new ARDocSetOffViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.ArDocSetOffHd Dochd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Dochd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Dochd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Dochd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Dochd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Dochd.CancelById WHERE (Dochd.SetoffNo LIKE '%{searchString}%' OR Dochd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND TrnDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<ARDocSetOffViewModel>($"SELECT Dochd.CompanyId,Dochd.SetoffId,Dochd.SetoffNo,Dochd.ReferenceNo,Dochd.TrnDate,Dochd.AccountDate,Dochd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Dochd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Dochd.ExhRate,Dochd.BalanceAmt,Dochd.AllocAmt,Dochd.Remarks,Dochd.UnAllocAmt,Dochd.ExhGainLoss,Dochd.ModuleFrom,Dochd.CreateById,Dochd.CreateDate,Dochd.EditById,Dochd.EditDate,Dochd.CancelById,Dochd.CancelDate,Dochd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Dochd.EditVersion FROM dbo.ArDocSetOffHd Dochd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Dochd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Dochd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Dochd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Dochd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Dochd.CancelById WHERE (Dochd.SetoffNo LIKE '%{searchString}%' OR Dochd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND Dochd.AccountDate BETWEEN '{fromDate}' AND '{toDate}' AND Invhd.CompanyId={CompanyId} ORDER BY Dochd.SetoffNo,Dochd.AccountDate OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.DocSetoff,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "ArDocSetOffHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ARDocSetOffViewModel> GetARDocSetOffByIdAsync(Int16 CompanyId, Int64 SetoffId, string SetoffNo, Int16 UserId)
        {
            ARDocSetOffViewModel ARDocSetOffViewModel = new ARDocSetOffViewModel();
            try
            {
                ARDocSetOffViewModel = await _repository.GetQuerySingleOrDefaultAsync<ARDocSetOffViewModel>($"SELECT Dochd.CompanyId,Dochd.SetoffId,Dochd.SetoffNo,Dochd.ReferenceNo,Dochd.TrnDate,Dochd.AccountDate,Dochd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Dochd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Dochd.ExhRate,Dochd.BalanceAmt,Dochd.AllocAmt,Dochd.UnAllocAmt,Dochd.ExhGainLoss,Dochd.Remarks,Dochd.ModuleFrom,Dochd.CreateById,Dochd.CreateDate,Dochd.EditById,Dochd.EditDate,Dochd.CancelById,Dochd.CancelDate,Dochd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Dochd.EditVersion FROM dbo.ArDocSetOffHd Dochd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Dochd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Dochd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Dochd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Dochd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Dochd.CancelById  WHERE (Dochd.SetoffId={SetoffId} OR {SetoffId}=0) AND (Dochd.SetoffNo='{SetoffNo}' OR '{SetoffNo}'='{string.Empty}')");

                if (ARDocSetOffViewModel == null)
                    return ARDocSetOffViewModel;

                var result = await _repository.GetQueryAsync<ARDocSetOffDtViewModel>($"SELECT Docdt.CompanyId,Docdt.SetoffId,Docdt.SetoffNo,Docdt.ItemNo,Docdt.TransactionId,Docdt.DocumentId,Docdt.DocumentNo,Docdt.ReferenceNo,Docdt.DocCurrencyId,Docdt.DocExhRate,Docdt.DocAccountDate,Docdt.DocDueDate,Docdt.DocTotAmt,Docdt.DocTotLocalAmt,Docdt.DocBalAmt,Docdt.DocBalLocalAmt,Docdt.AllocAmt,Docdt.AllocLocalAmt,Docdt.DocAllocAmt,Docdt.DocAllocLocalAmt,Docdt.CentDiff,Docdt.ExhGainLoss FROM dbo.ArDocSetOffDt Docdt  WHERE Docdt.SetoffId={ARDocSetOffViewModel.SetoffId}");

                ARDocSetOffViewModel.data_details = result == null ? null : result.ToList();

                return ARDocSetOffViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.DocSetoff,
                    DocumentId = SetoffId,
                    DocumentNo = SetoffNo,
                    TblName = "ArDocSetOffHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveARDocSetOffAsync(Int16 CompanyId, ArDocSetOffHd arDocSetOffHd, List<ArDocSetOffDt> arDocSetOffDts, Int16 UserId)
        {
            bool IsEdit = false;
            string accountDate = arDocSetOffHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //var IsPeriodClosed = await _repository.GetQuerySingleOrDefaultAsync<bool>( $"SELECT dbo.CheckPeriodClosed({CompanyId},{(short)E_Modules.AR},'{accountDate}') as IsExist");

                    //if (!IsPeriodClosed)
                    //{
                    if (arDocSetOffHd.SetoffId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.ArDocSetOffHd WHERE IsCancel=0 And CompanyId={CompanyId} And SetoffId={arDocSetOffHd.SetoffId}");

                        if (dataExist.Count() == 0)
                            return new SqlResponse { Result = -1, Message = "DocSetOff Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponseIds>($"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.AR},{(short)E_AR.DocSetoff},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            arDocSetOffHd.SetoffId = documentIdNo.ToList()[0].DocumentId;
                            arDocSetOffHd.SetoffNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        //Insert the previous ARDocSetOff record to ARDocSetOff history table as well as editversion also.
                        await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AR_CreateHistoryRec {CompanyId},{UserId},{arDocSetOffHd.SetoffId},{(short)E_AR.DocSetoff}");
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
                            _context.ArDocSetOffDt.Where(x => x.SetoffId == arDocSetOffHd.SetoffId).ExecuteDelete();

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
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arDocSetOffHd.SetoffId},{(short)E_AR.DocSetoff}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AR,
                                TransactionId = (short)E_AR.DocSetoff,
                                DocumentId = arDocSetOffHd.SetoffId,
                                DocumentNo = arDocSetOffHd.SetoffNo,
                                TblName = "ArDocSetOffHd",
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
                                    await _repository.UpsertExecuteScalarAsync($"update ArDocSetOffHd set EditVersion=EditVersion+1 where SetoffId={arDocSetOffHd.SetoffId}; Update ArDocSetOffDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.ArDocSetOffHd where SetoffId={arDocSetOffHd.SetoffId}) where SetoffId={arDocSetOffHd.SetoffId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arDocSetOffHd.SetoffId},{(short)E_AR.DocSetoff}");

                                TScope.Complete();
                                return new SqlResponse { Result = arDocSetOffHd.SetoffId, Message = "Save Successfully" };
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
                    TransactionId = (short)E_AR.DocSetoff,
                    DocumentId = arDocSetOffHd.SetoffId,
                    DocumentNo = arDocSetOffHd.SetoffNo,
                    TblName = "ArDocSetOffHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponse> DeleteARDocSetOffAsync(Int16 CompanyId, Int64 SetoffId, string SetoffNo, string CanacelRemarks, Int16 UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (SetoffId > 0)
                    {
                        var ARDocSetOffToRemove = _context.ArDocSetOffHd.Where(b => b.SetoffId == SetoffId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (ARDocSetOffToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponseIds>($"exec FIN_AR_DeleteStatement {CompanyId},{UserId},{SetoffId},{(short)E_AR.DocSetoff}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AR,
                                TransactionId = (short)E_AR.DocSetoff,
                                DocumentId = SetoffId,
                                DocumentNo = SetoffNo,
                                TblName = "ArDocSetOffHd",
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
                        return new SqlResponse { Result = -1, Message = "DocSetOff Not exists" };
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
                    TransactionId = (short)E_AR.DocSetoff,
                    DocumentId = SetoffId,
                    DocumentNo = SetoffNo,
                    TblName = "ArDocSetOffHd",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<ARDocSetOffViewModel>> GetHistoryARDocSetOffByIdAsync(Int16 CompanyId, Int64 SetoffId, string SetoffNo, Int16 UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<ARDocSetOffViewModel>($"SELECT Dochd.CompanyId,Dochd.SetoffId,Dochd.SetoffNo,Dochd.ReferenceNo,Dochd.TrnDate,Dochd.AccountDate,Dochd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Dochd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Dochd.ExhRate,Dochd.BalanceAmt,Dochd.AllocAmt,Dochd.UnAllocAmt,Dochd.ExhGainLoss,Dochd.ModuleFrom,Dochd.CreateById,Dochd.CreateDate,Dochd.EditById,Dochd.EditDate,Dochd.CancelById,Dochd.CancelDate,Dochd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy,Dochd.EditVersion FROM dbo.ArDocSetOffHd_Ver Dochd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Dochd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Dochd.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Dochd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Dochd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Dochd.CancelById  WHERE (Dochd.SetoffId={SetoffId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.DocSetoff,
                    DocumentId = SetoffId,
                    DocumentNo = SetoffNo,
                    TblName = "ArDocSetOffHd",
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