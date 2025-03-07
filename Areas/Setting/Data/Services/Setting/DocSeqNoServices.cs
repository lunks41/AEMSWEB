using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Enums;
using AEMSWEB.Helper;
using AEMSWEB.IServices;
using AEMSWEB.IServices.Setting;
using AEMSWEB.Models;
using AEMSWEB.Repository;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace AEMSWEB.Services.Setting
{
    public sealed class DocSeqNoServices : IDocSeqNoService
    {
        private readonly IRepository<S_DocSeqNo> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public DocSeqNoServices(IRepository<S_DocSeqNo> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<DocSeqNoViewModel> GetDocSeqNoByTransactionAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DocSeqNoViewModel>($"SELECT CompanyId,ModuleId,TransactionId,H_ReferenceNo,H_TrnDate,H_AccountDate,H_DeliveryDate,H_DueDate,H_CustomerId,H_CurrencyId,H_ExhRate,H_CtyExhRate,H_CreditTermId,H_DocSeqNoId,H_InvoiceNo,H_TotAmt,H_TotLocalAmt,H_TotCtyAmt,H_GstClaimDate,H_GstAmt,H_GstLocalAmt,H_GstCtyAmt,H_TotAmtAftGst,H_TotLocalAmtAftGst,H_TotCtyAmtAftGst,H_SalesOrderNo,H_OperationNo,H_Remarks,H_Address1,H_Address2,H_Address3,H_Address4,H_PinCode,H_CountryId,H_PhoneNo,H_FaxNo,H_ContactName,H_MobileNo,H_EmailAdd,H_SupplierName,H_SuppInvoiceNo,H_APInvoiceNo,D_SeqNo,D_ProductId,D_GLId,D_QTY,D_BillQTY,D_UomId,D_UnitPrice,D_TotAmt,D_TotLocalAmt,D_TotCtyAmt,D_Remarks,D_GstId,D_GstPercentage,D_GstAmt,D_GstLocalAmt,D_GstCtyAmt,D_DeliveryDate,D_DepartmentId,D_EmployeeId,D_PortId,D_VesselId,D_BargeId,D_VoyageId,D_OperationNo,D_OPRefNo,D_SalesOrderNo,D_SupplyDate,D_SupplierName,D_SuppInvoiceNo,D_APInvoiceNo,CreateById,CreateDate,EditById,EditDate FROM S_DocSeqNo where ModuleId={ModuleId} And TransactionId={TransactionId} And CompanyId in (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Setting},{(short)E_Setting.DocSeqNo}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocSeqNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_DocSeqNo",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<DocSeqNoViewModel> GetDocSeqNoAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DocSeqNoViewModel>($"SELECT DocSeq.CompanyId,DocSeq.ModuleId,DocSeq.TransactionId,H_ReferenceNo,H_TrnDate,H_AccountDate,H_DeliveryDate,H_DueDate,H_CustomerId,H_CurrencyId,H_ExhRate,H_CtyExhRate,H_CreditTermId,H_DocSeqNoId,H_InvoiceNo,H_TotAmt,H_TotLocalAmt,H_TotCtyAmt,H_GstClaimDate,H_GstAmt,H_GstLocalAmt,H_GstCtyAmt,H_TotAmtAftGst,H_TotLocalAmtAftGst,H_TotCtyAmtAftGst,H_SalesOrderNo,H_OperationNo,H_Remarks,H_Address1,H_Address2,H_Address3,H_Address4,H_PinCode,H_CountryId,H_PhoneNo,H_FaxNo,H_ContactName,H_MobileNo,H_EmailAdd,H_SupplierName,H_SuppInvoiceNo,H_APInvoiceNo,D_SeqNo,D_ProductId,D_GLId,D_QTY,D_BillQTY,D_UomId,D_UnitPrice,D_TotAmt,D_TotLocalAmt,D_TotCtyAmt,D_Remarks,D_GstId,D_GstPercentage,D_GstAmt,D_GstLocalAmt,D_GstCtyAmt,D_DeliveryDate,D_DepartmentId,D_EmployeeId,D_PortId,D_VesselId,D_BargeId,D_VoyageId,D_OperationNo,D_OPRefNo,D_SalesOrderNo,D_SupplyDate,D_SupplierName,D_SuppInvoiceNo,D_APInvoiceNo,DocSeq.CreateById,Usr.UserCode CreateBy, DocSeq.CreateDate, DocSeq.EditById,Usr1.UserCode EditBy,DocSeq.EditDate FROM S_DocSeqNo DocSeq Left Join AdmUser Usr on Usr.UserId=DocSeq.CreatebyId Left Join AdmUser Usr1 on Usr1.UserId=DocSeq.CreatebyId where CompanyId in (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Setting},{(short)E_Setting.DocSeqNo}))");
                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocSeqNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_DocSeqNo",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> UpsertDocSeqNoAsync(Int16 CompanyId, S_DocSeqNo s_DocSeqNo, Int16 UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM S_DocSeqNo WHERE ModuleId={s_DocSeqNo.ModuleId} And TransactionId={s_DocSeqNo.TransactionId} And CompanyId in (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Setting},{(short)E_Setting.DocSeqNo}))");

                    if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                    {
                        var entity = _context.Update(s_DocSeqNo);
                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        s_DocSeqNo.EditById = null;
                        s_DocSeqNo.EditDate = null;
                        _context.Add(s_DocSeqNo);
                    }

                    var FinSettingsToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (FinSettingsToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.DocSeqNo,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_DocSeqNo",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "Document Seqence Settings Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.DocSeqNo, 0, "", "S_DocSeqNo", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.DocSeqNo, 0, "", "S_DocSeqNo", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }
    }
}