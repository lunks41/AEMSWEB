using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace AEMSWEB.Areas.Master.Data.Services
{
    public sealed class ProductService : IProductService
    {
        private readonly IRepository<M_Product> _repository;
        private ApplicationDbContext _context;

        public ProductService(IRepository<M_Product> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<ProductViewModelCount> GetProductListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            ProductViewModelCount countViewModel = new ProductViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Product M_Prod WHERE (M_Prod.ProductName LIKE '%{searchString}%' OR M_Prod.ProductCode LIKE '%{searchString}%' OR M_Prod.Remarks LIKE '%{searchString}%') AND M_Prod.ProductId<>0 AND M_Prod.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Product}))");

                var result = await _repository.GetQueryAsync<ProductViewModel>($"SELECT M_Prod.ProductId,M_Prod.ProductCode,M_Prod.ProductName,M_Prod.CompanyId,M_Prod.Remarks,M_Prod.IsActive,M_Prod.CreateById,M_Prod.CreateDate,M_Prod.EditById,M_Prod.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Product M_Prod LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Prod.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Prod.EditById WHERE (M_Prod.ProductName LIKE '%{searchString}%' OR M_Prod.ProductCode LIKE '%{searchString}%' OR M_Prod.Remarks LIKE '%{searchString}%') AND M_Prod.ProductId<>0 AND M_Prod.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Product})) ORDER BY M_Prod.ProductName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result == null ? null : result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Product,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Product",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Product> GetProductByIdAsync(short CompanyId, short UserId, short ProductId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Product>($"SELECT ProductId,CompanyId,ProductCode,ProductName,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Product WHERE ProductId={ProductId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Product}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Product,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Product",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveProductAsync(short CompanyId, short UserId, M_Product m_Product)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Product.ProductId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Product WHERE ProductId<>@ProductId AND ProductCode=@ProductCode",
                        new { m_Product.ProductId, m_Product.ProductCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Product Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Product WHERE ProductId<>@ProductId AND ProductName=@ProductName",
                        new { m_Product.ProductId, m_Product.ProductName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Product Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Product WHERE ProductId=@ProductId",
                            new { m_Product.ProductId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Product);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Product Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (ProductId + 1) FROM dbo.M_Product WHERE (ProductId + 1) NOT IN (SELECT ProductId FROM dbo.M_Product)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Product.ProductId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Product);
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Product,
                            DocumentId = m_Product.ProductId,
                            DocumentNo = m_Product.ProductCode,
                            TblName = "M_Product",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Product Save Successfully",
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
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Product,
                        DocumentId = m_Product.ProductId,
                        DocumentNo = m_Product.ProductCode,
                        TblName = "M_Product",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException?.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw;
                }
            }
        }

        public async Task<SqlResponse> DeleteProductAsync(short CompanyId, short UserId, M_Product Product)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (Product.ProductId > 0)
                    {
                        var ProductToRemove = _context.M_Product.Where(x => x.ProductId == Product.ProductId).ExecuteDelete();

                        if (ProductToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Product,
                                DocumentId = Product.ProductId,
                                DocumentNo = Product.ProductCode,
                                TblName = "M_Product",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Product Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "ProductId Should be zero" };
                    }
                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Product,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Product",
                        ModeId = (short)E_Mode.Delete,
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
}