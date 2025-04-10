using AMESWEB.Areas.Account.Data.IServices;
using AMESWEB.Areas.Account.Data.IServices.GL;
using AMESWEB.Areas.Account.Models.GL;
using AMESWEB.Data;
using AMESWEB.Entities.Accounts.GL;
using AMESWEB.Entities.Admin;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Models;
using AMESWEB.Repository;
using System.Transactions;

namespace AMESWEB.Areas.Account.Data.Services.GL
{
    public sealed class GLOpeningBalanceService : IGLOpeningBalanceService
    {
        private readonly IRepository<GLOpeningBalance> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public GLOpeningBalanceService(IRepository<GLOpeningBalance> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<IEnumerable<GLOpeningBalanceViewModel>> GetGLOpeningBalanceListAsync(short CompanyId, int DocumentId, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<GLOpeningBalanceViewModel>($"SELECT Glopn.CompanyId,Glopn.DocumentId,Glopn.ItemNo,Glopn.GLId,M_CharAcc.GLCode,M_CharAcc.GLName,Glopn.DocumentNo,Glopn.AccountDate,Glopn.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Glopn.SupplierId,M_sup.SupplierCode,M_sup.SupplierName,Glopn.CurrencyId,M_Curr.CurrencyCode,M_Curr.CurrencyName,Glopn.ExhRate,Glopn.IsDebit,Glopn.TotAmt,Glopn.TotLocalAmt,Glopn.DepartmentId,M_dep.DepartmentCode,M_dep.DepartmentName,Glopn.EmployeeId,M_Emp.EmployeeCode,M_Emp.EmployeeName,Glopn.ProductId,M_Pro.ProductCode,M_Pro.ProductName,Glopn.PortId,M_por.PortCode,M_Pro.ProductName,Glopn.VesselId,M_vel.VesselCode,M_vel.VesselName,Glopn.BargeId,M_brg.BargeCode,M_brg.BargeName,Glopn.VoyageId,M_Voy.VoyageNo,Glopn.CreateById,Glopn.CreateDate,Glopn.EditById,Glopn.EditDate FROM dbo.GLOpeningBalance Glopn INNER JOIN dbo.M_Currency M_Curr ON M_Curr.CurrencyId = Glopn.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_CharAcc ON M_CharAcc.GLId = Glopn.GLId LEFT JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Glopn.CustomerId LEFT JOIN dbo.M_Supplier M_sup ON M_sup.SupplierId = Glopn.SupplierId LEFT JOIN dbo.M_Department M_dep ON M_dep.DepartmentId = Glopn.DepartmentId LEFT JOIN dbo.M_Employee M_Emp ON M_Emp.EmployeeId = Glopn.EmployeeId LEFT JOIN dbo.M_Product M_Pro ON M_Pro.ProductId = Glopn.ProductId LEFT JOIN dbo.M_Port M_por ON M_por.PortId = Glopn.PortId LEFT JOIN dbo.M_Vessel M_vel ON M_vel.VesselId = Glopn.VesselId LEFT JOIN dbo.M_Barge M_brg ON M_brg.BargeId = Glopn.BargeId LEFT JOIN dbo.M_Voyage M_Voy ON M_Voy.VoyageId = Glopn.VoyageId WHERE Glopn.DocumentId={DocumentId}");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.OpeningBalance,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GLOpeningBalance",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveGLOpeningBalanceAsync(short CompanyId, GLOpeningBalanceViewModel gLOpeningBalanceViewModel, short UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (gLOpeningBalanceViewModel.DocumentId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.GLOpeningBalance WHERE CompanyId IN {CompanyId} AND DocumentId={gLOpeningBalanceViewModel.DocumentId} AND ItemNo={gLOpeningBalanceViewModel.ItemNo}");

                        if (dataExist.Count() > 0 && (dataExist.ToList()[0].IsExist == 1 || dataExist.ToList()[0].IsExist == 2))
                            return new SqlResponce { Result = -1, Message = "ItemNo Exist" };
                    }
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(gLOpeningBalanceViewModel);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT ISNULL((SELECT TOP 1 (ItemNo + 1) FROM dbo.GLOpeningBalance WHERE (ItemNo + 1) NOT IN (SELECT ItemNo FROM dbo.GLOpeningBalance) AND DocumentId={gLOpeningBalanceViewModel.DocumentId} AND CompanyId={CompanyId}),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            gLOpeningBalanceViewModel.ItemNo = Convert.ToInt32(sqlMissingResponse.NextId);
                            gLOpeningBalanceViewModel.EditDate = null;
                            gLOpeningBalanceViewModel.EditById = null;
                            _context.Add(gLOpeningBalanceViewModel);
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "ItemNo Should not be zero" };
                    }

                    var SupplierToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (SupplierToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.GL,
                            TransactionId = (short)E_GL.OpeningBalance,
                            DocumentId = gLOpeningBalanceViewModel.DocumentId,
                            DocumentNo = gLOpeningBalanceViewModel.ItemNo.ToString(),
                            TblName = "GLOpeningBalance",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Supplier Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponce { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.GL,
                        TransactionId = (short)E_GL.OpeningBalance,
                        DocumentId = gLOpeningBalanceViewModel.DocumentId,
                        DocumentNo = gLOpeningBalanceViewModel.ItemNo.ToString(),
                        TblName = "GLOpeningBalance",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException?.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<IEnumerable<GLOpeningBalanceViewModel>> GenerateGLYearEndProcessAsyn(short CompanyId, int DocumentId, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<GLOpeningBalanceViewModel>($"exec Adm_GetUserTransactions {CompanyId},{DocumentId},{UserId}");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.YearEndProcess,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "YearEndProcess",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
    }
}