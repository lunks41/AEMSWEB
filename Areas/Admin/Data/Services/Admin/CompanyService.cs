using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;
using AEMSWEB.Repository;

namespace AEMSWEB.Services.Admin
{
    public sealed class CompanyService : ICompanyService
    {
        private readonly IRepository<AdmCompany> _repository;
        private ApplicationDbContext _context;

        public CompanyService(IRepository<AdmCompany> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<CompanyViewModel>> GetUserCompanyListAsync(Int16 UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<CompanyViewModel>($"SELECT CompanyId,CompanyName FROM AdmCompany WHERE IsActive=1 AND CompanyId IN (SELECT CompanyId FROM AdmUserRights WHERE UserId={UserId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetUserLoginCompany",
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