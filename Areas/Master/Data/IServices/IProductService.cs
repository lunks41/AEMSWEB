﻿using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IProductService
    {
        public Task<ProductViewModelCount> GetProductListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<ProductViewModel> GetProductByIdAsync(short CompanyId, short UserId, short ProductId);

        public Task<SqlResponse> SaveProductAsync(short CompanyId, short UserId, M_Product M_Product);

        public Task<SqlResponse> DeleteProductAsync(short CompanyId, short UserId, short ProductId);
    }
}