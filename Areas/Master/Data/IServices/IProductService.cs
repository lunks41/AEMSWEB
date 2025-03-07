using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IProductService
    {
        public Task<ProductViewModelCount> GetProductListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_Product> GetProductByIdAsync(short CompanyId, short UserId, short ProductId);

        public Task<SqlResponse> SaveProductAsync(short CompanyId, short UserId, M_Product M_Product);

        public Task<SqlResponse> DeleteProductAsync(short CompanyId, short UserId, M_Product M_Product);
    }
}