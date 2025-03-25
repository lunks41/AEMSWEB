using AEMSWEB.Data;
using AEMSWEB.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AEMSWEB.Components
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NavigationViewComponent(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return Content(string.Empty);
            }

            // Get user/company IDs from claims
            var companyId = user.FindFirst("CompanyId")?.Value ?? "1";
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!short.TryParse(companyId, out var companyIdShort) ||
                !short.TryParse(userId, out var userIdShort))
            {
                return Content(string.Empty);
            }

            using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            var rights = await connection.QueryAsync<TransactionViewModel>(
                "EXEC [dbo].[Adm_GetUserTransactions_All] @inCompanyId, @inUserId",
                new { inCompanyId = companyIdShort, inUserId = userIdShort });

            if (rights != null && rights.Any())
            {
                var modules = rights
                    .GroupBy(t => new { t.ModuleId, t.ModuleCode, t.ModuleName, t.ModuleSeqNo })
                    .Select(g => new ModuleView
                    {
                        ModuleId = g.Key.ModuleId,
                        ModuleCode = g.Key.ModuleCode,
                        ModuleName = g.Key.ModuleName,
                        ModuleSeqNo = g.Key.ModuleSeqNo,
                        TransCategorys = g.GroupBy(tc => new { tc.TransCategoryId, tc.TransCategoryCode, tc.TransCategoryName })
                            .Select(tcg => new TransCategoryView
                            {
                                TransCategoryId = tcg.Key.TransCategoryId,
                                TransCategoryCode = tcg.Key.TransCategoryCode,
                                TransCategoryName = tcg.Key.TransCategoryName,
                                Transactions = tcg.Select(t => new TransactionView
                                {
                                    TransactionId = t.TransactionId,
                                    TransactionCode = t.TransactionCode,
                                    TransactionName = t.TransactionName,
                                    IsCreate = t.IsCreate,
                                    IsEdit = t.IsEdit,
                                    IsDelete = t.IsDelete,
                                    IsExport = t.IsExport,
                                    IsPrint = t.IsPrint
                                }).ToList()
                            }).ToList()
                    }).ToList();

                ViewBag.Modules = modules;

                return View(modules);
            }

            return Content(string.Empty);
        }

        //public class NavigationViewComponent : ViewComponent
        //{
        //    private readonly ApplicationDbContext _context;

        //    public NavigationViewComponent(ApplicationDbContext context)
        //    {
        //        _context = context;
        //    }
        // [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
        //    public async Task<IViewComponentResult> InvokeAsync()
        //    {
        //        var modules = await _context.AdmModule
        //            .Include(m => m.Transactions)
        //                .ThenInclude(t => t.TransCategory)
        //            .OrderBy(m => m.SeqNo)
        //            .ToListAsync();

        //        return View(modules);
        //    }
        //}

        //var rights = await connection.QueryAsync<UserTransactionRights>(
        //       "EXEC [dbo].[Adm_GetUserTransactions_All] @inCompanyId, @inUserId",
        //       new { inCompanyId = companyId, inUserId = userId });

        //// Extract allowed transaction IDs
        //var allowedTransactionIds = rights.Select(r => r.TransactionId).ToHashSet();

        //var modules = await _context.AdmModule
        //    .Include(m => m.Transactions)
        //        .ThenInclude(t => t.TransCategory)
        //    .OrderBy(m => m.SeqNo)
        //    .ToListAsync();
    }
}