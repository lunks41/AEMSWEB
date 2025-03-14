using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.Models.Admin;
using AEMSWEB.Repository;

namespace AEMSWEB.Areas.Admin.Data
{
    public sealed class ModuleService : IModuleService
    {
        private readonly IRepository<UserModuleViewModel> _repository;
        private ApplicationDbContext _context;

        public ModuleService(IRepository<UserModuleViewModel> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<UserModuleViewModel>> GetUsersModulesAsync(short CompanyId, short UserId)
        {
            try
            {
                var productDetails = await _repository.GetQueryAsync<UserModuleViewModel>($"exec Adm_GetUserModules {CompanyId},{UserId}");

                return productDetails;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Modules,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetUsersModulesAsync",
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