using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Enums;
using AMESWEB.Models.Admin;
using AMESWEB.Repository;
using Dapper;

namespace AMESWEB.Areas.Admin.Data
{
    public sealed class TransactionService : ITransactionService
    {
        private readonly IRepository<TransactionViewModel> _repository;
        private ApplicationDbContext _context;

        public TransactionService(IRepository<TransactionViewModel> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<TransactionViewModel>> GetUsersTransactionsAsync(short CompanyId, short ModuleId, short UserId)
        {
            var parameters = new DynamicParameters();
            try
            {
                //var parameter = new List<SqlParameter>();
                //parameter.Add(new SqlParameter("@CompanyId", CompanyId));
                //parameter.Add(new SqlParameter("@ModuleId", ModuleId));
                //parameter.Add(new SqlParameter("@UserId", UserId));

                var productDetails = await _repository.GetQueryAsync<TransactionViewModel>($"exec Adm_GetUserTransactions {CompanyId},{ModuleId},{UserId}");

                //var productDetails = await Task.Run(() => _context.AdmTransaction
                //                .FromSqlRaw(@"exec Adm_GetUserTransactions @CompanyId,@ModuleId,@UserId", parameter.ToArray()).ToListAsync());

                return productDetails;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Transaction,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetUsersTransactionsAsync",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<TransactionViewModel>> GetUsersTransactionsAllAsync(short CompanyId, short UserId)
        {
            var parameters = new DynamicParameters();
            try
            {
                var productDetails01 = await _repository.GetQueryAsync<dynamic>($"exec Adm_GetUserTransactions_All {CompanyId},{UserId}");
                var productDetails = await _repository.GetQueryAsync<TransactionViewModel>($"exec Adm_GetUserTransactions_All {CompanyId},{UserId}");

                return productDetails;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Transaction,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetUsersTransactionsAllAsync",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<TransactionViewModel>> GetUsersTransactionsAllAsyncV1(short CompanyId, short UserId)
        {
            var parameters = new DynamicParameters();
            List<GroupViewModel> groupViewModelsList = new List<GroupViewModel>();
            GroupViewModel groupViewModels = new GroupViewModel();

            try
            {
                var moduleDetails = await _repository.GetQueryAsync<TransactionViewModel>($"SELECT M.ModuleId, M.ModuleId,M.ModuleCode FROM dbo.AdmUserGroupRights UGR Inner Join AdmUser U on U.UserGroupId=UGR.UserGroupId INNER JOIN AdmUserRights UR ON UR.UserId=U.UserId INNER Join AdmModule M on M.ModuleId=UGR.ModuleId WHERE UR.CompanyId={CompanyId} AND UR.UserId={UserId} GROUP BY M.ModuleId, M.ModuleId,M.ModuleCode");

                var transCateogryDetails = await _repository.GetQueryAsync<TransactionViewModel>($"SELECT M.ModuleId, TC.TransCategoryId, TC.TransCategoryCode,TC.TransCategoryName FROM dbo.AdmUserGroupRights UGR Inner Join AdmUser U on U.UserGroupId=UGR.UserGroupId INNER JOIN AdmUserRights UR ON UR.UserId=U.UserId INNER Join AdmModule M on M.ModuleId=UGR.ModuleId Inner Join AdmTransaction T on T.TransactionId=UGR.TransactionId And T.ModuleId=UGR.ModuleId Inner Join AdmTransactionCategory TC on TC.TransCategoryId=T.TransCategoryId AND TC.TransCategoryId<>0 WHERE UR.CompanyId={CompanyId} AND UR.UserId={UserId} GROUP BY M.ModuleId,TC.TransCategoryId,TC.TransCategoryCode,TC.TransCategoryName");

                var transactionDetails = await _repository.GetQueryAsync<TransactionViewModel>($"SELECT M.ModuleId,TC.TransCategoryId, T.TransactionId, T.TransactionCode,T.TransactionName FROM dbo.AdmUserGroupRights UGR Inner Join AdmUser U on U.UserGroupId=UGR.UserGroupId INNER JOIN AdmUserRights UR ON UR.UserId=U.UserId Inner Join AdmModule M on M.ModuleId=UGR.ModuleId Inner Join AdmTransaction T on T.TransactionId=UGR.TransactionId And T.ModuleId=UGR.ModuleId INNER Join AdmTransactionCategory TC on TC.TransCategoryId=T.TransCategoryId WHERE UR.CompanyId={CompanyId} AND UR.UserId={UserId} GROUP BY M.ModuleId,TC.TransCategoryId,T.TransactionId,T.TransactionCode,T.TransactionName");

                return moduleDetails;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Transaction,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetUsersTransactionsAllAsync",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<List<GroupViewModel>> GetMenuListAsync(short CompanyId, short UserId)
        {
            // Fetch module details
            var moduleDetails = await _repository.GetQueryAsync<TransactionViewModel>(
                $"SELECT M.ModuleId, M.ModuleCode FROM dbo.AdmUserGroupRights UGR " +
                "Inner Join AdmUser U on U.UserGroupId=UGR.UserGroupId " +
                "INNER JOIN AdmUserRights UR ON UR.UserId=U.UserId " +
                "INNER Join AdmModule M on M.ModuleId=UGR.ModuleId " +
                $"WHERE UR.CompanyId={CompanyId} AND UR.UserId={UserId} GROUP BY M.ModuleId, M.ModuleCode");

            // Fetch transaction category details
            var transCategoryDetails = await _repository.GetQueryAsync<TransactionViewModel>(
                $"SELECT M.ModuleId, TC.TransCategoryId, TC.TransCategoryCode, TC.TransCategoryName FROM dbo.AdmUserGroupRights UGR " +
                "Inner Join AdmUser U on U.UserGroupId=UGR.UserGroupId " +
                "INNER JOIN AdmUserRights UR ON UR.UserId=U.UserId " +
                "INNER Join AdmModule M on M.ModuleId=UGR.ModuleId " +
                "Inner Join AdmTransaction T on T.TransactionId=UGR.TransactionId AND T.ModuleId=UGR.ModuleId " +
                "Inner Join AdmTransactionCategory TC on TC.TransCategoryId=T.TransCategoryId AND TC.TransCategoryId<>0 " +
                $"WHERE UR.CompanyId={CompanyId} AND UR.UserId={UserId} GROUP BY M.ModuleId, TC.TransCategoryId, TC.TransCategoryCode, TC.TransCategoryName");

            // Fetch transaction details
            var transactionDetails = await _repository.GetQueryAsync<TransactionViewModel>(
                $"SELECT M.ModuleId, TC.TransCategoryId, T.TransactionId, T.TransactionCode, T.TransactionName FROM dbo.AdmUserGroupRights UGR " +
                "Inner Join AdmUser U on U.UserGroupId=UGR.UserGroupId " +
                "INNER JOIN AdmUserRights UR ON UR.UserId=U.UserId " +
                "INNER Join AdmModule M on M.ModuleId=UGR.ModuleId " +
                "Inner Join AdmTransaction T on T.TransactionId=UGR.TransactionId AND T.ModuleId=UGR.ModuleId " +
                "INNER Join AdmTransactionCategory TC on TC.TransCategoryId=T.TransCategoryId " +
                $"WHERE UR.CompanyId={CompanyId} AND UR.UserId={UserId} GROUP BY M.ModuleId, TC.TransCategoryId, T.TransactionId, T.TransactionCode, T.TransactionName");

            // Initialize the final list to hold GroupViewModel
            var menuList = new List<GroupViewModel>();

            // Step 1: Loop through moduleDetails and create GroupViewModel
            foreach (var module in moduleDetails)
            {
                var group = new GroupViewModel
                {
                    groupLabel = module.ModuleCode,
                    id = module.ModuleId.ToString(),
                    menus = new List<MenuViewModel>()
                };

                // Step 2: For each module, find corresponding transaction categories
                var moduleCategories = transCategoryDetails
                    .Where(tc => tc.ModuleId == module.ModuleId)
                    .ToList();

                foreach (var category in moduleCategories)
                {
                    var menu = new MenuViewModel
                    {
                        href = $"/{module.ModuleCode}/{category.TransCategoryCode}",
                        label = category.TransCategoryName,
                        active = false, // You can set the active condition based on your logic
                        icon = "default-icon", // Set default icon or custom logic to select icon
                        id = category.TransCategoryId.ToString(),
                        submenus = new List<SubMenuViewModel>()
                    };

                    // Step 3: For each transaction category, find corresponding transactions
                    var categoryTransactions = transactionDetails
                        .Where(t => t.ModuleId == module.ModuleId && t.TransCategoryId == category.TransCategoryId)
                        .ToList();

                    foreach (var transaction in categoryTransactions)
                    {
                        var submenu = new SubMenuViewModel
                        {
                            href = $"/{module.ModuleCode}/{category.TransCategoryCode}/{transaction.TransactionCode}",
                            label = transaction.TransactionName,
                            active = false, // You can set the active condition based on your logic
                            icon = "default-sub-icon" // Set default icon or custom logic
                        };

                        menu.submenus.Add(submenu);
                    }

                    group.menus.Add(menu);
                }

                menuList.Add(group);
            }

            return menuList;
        }
    }
}