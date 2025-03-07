using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices.Setting;
using AEMSWEB.Repository;

namespace AEMSWEB.Services.Setting
{
    public sealed class BaseSettingsServices : IBaseSettingsService
    {
        private readonly IRepository<dynamic> _repository;
        private ApplicationDbContext _context;

        public BaseSettingsServices(IRepository<dynamic> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<decimal> GetExchangeRateAsync(Int16 CompanyId, Int16 CurrencyId, string TrnsDate, Int16 UserId)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>($"select dbo.GetExchangeRate ({CompanyId},{CurrencyId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<decimal> GetExchangeRateLocalAsync(Int16 CompanyId, Int16 CurrencyId, string TrnsDate, Int16 UserId)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>($"select dbo.GetExchangeRate_Local ({CompanyId},{CurrencyId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<bool> GetCheckPeriodClosedAsync(Int16 CompanyId, Int16 ModuleId, string TrnsDate, Int16 UserId)
        {
            try
            {
                bool IsResult = await _repository.GetQuerySingleOrDefaultAsync<bool>($"select  dbo.CheckPeriodClosed ({CompanyId},{ModuleId},'{TrnsDate}')");

                return IsResult;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<decimal> GetGstPercentageAsync(Int16 CompanyId, Int16 GstId, string TrnsDate, Int16 UserId)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>($"select dbo.GetGSTPercentage ({CompanyId},{GstId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<decimal> GetCreditTermDayAsync(Int16 CompanyId, Int16 CreditTermId, string TrnsDate, Int16 UserId)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>($"select dbo.GetCreditTermDays ({CompanyId},{CreditTermId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
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