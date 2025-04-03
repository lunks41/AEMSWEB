using AMESWEB.Areas.Setting.Models;
using AMESWEB.Entities.Setting;
using AMESWEB.Models;

namespace AMESWEB.Areas.Setting.Data
{
    public interface ISettingService
    {
        #region

        public Task<DecimalSettingViewModel> GetDecSettingAsync(short CompanyId, short UserId);

        public Task<SqlResponse> SaveDecSettingAsync(short CompanyId, short UserId, S_DecSettings s_DecSettings);

        #endregion

        #region

        public Task<FinanceSettingViewModel> GetFinSettingAsync(short CompanyId, short UserId);

        public Task<SqlResponse> SaveFinSettingAsync(short CompanyId, short UserId, S_FinSettings s_FinSettings);

        #endregion

        #region

        public Task<IEnumerable<MandatoryFieldsViewModel>> GetMandatoryFieldsByIdAsync(short CompanyId, short UserId, Int16 ModuleId);

        public Task<SqlResponse> SaveMandatoryFieldsAsync(short CompanyId, short UserId, List<S_MandatoryFields> s_MandatoryFields);

        #endregion

        #region

        public Task<VisibleFieldsViewModel> GetVisibleFieldsByIdAsync(short CompanyId, short UserId, Int16 ModuleId, Int16 TransactionId);

        public Task<IEnumerable<VisibleFieldsViewModel>> GetVisibleFieldsByIdAsync(short CompanyId, short UserId, Int16 ModuleId);

        public Task<SqlResponse> SaveVisibleFieldsAsync(short CompanyId, short UserId, List<S_VisibleFields> s_VisibleFields);

        #endregion

        #region

        public Task<ModelNameViewModelCount> GetNumberFormatListAsync(short CompanyId, short UserId);

        public Task<NumberSettingViewModel> GetNumberFormatByIdAsync(short CompanyId, short UserId, Int32 ModuleId, Int32 TransactionId);

        public Task<NumberSettingDtViewModel> GetNumberFormatByYearAsync(short CompanyId, short UserId, Int32 NumberId, Int32 NumYear);

        public Task<SqlResponse> SaveNumberFormatAsync(short CompanyId, short UserId, S_NumberFormat s_NumberFormat);

        #endregion

        #region

        public Task<UserGridViewModelCount> GetUserGridAsync(short CompanyId, short UserId, Int16 ModuleId, Int16 TransactionId);

        public Task<UserGridViewModel> GetUserGridByIdAsync(short CompanyId, short UserId, UserGridViewModel userGridViewModel);

        public Task<IEnumerable<UserGridViewModel>> GetUserGridByUserIdAsync(short CompanyId, short UserId, Int16 SelectedUserId);

        public Task<SqlResponse> SaveUserGridAsync(short CompanyId, short UserId, S_UserGrdFormat s_UserGrdFormat);

        public Task<SqlResponse> CloneUserGridSettingAsync(short CompanyId, short UserId, Int16 FromUserId, Int16 ToUserId);

        public Task<UserSettingViewModel> GetUserSettingAsync(short CompanyId, short UserId);

        public Task<SqlResponse> SaveUserSettingAsync(short CompanyId, short UserId, S_UserSettings S_UserSettings);

        #endregion

        public Task<decimal> GetExchangeRateAsync(short CompanyId, short UserId, Int16 CurrencyId, DateTime? TrnsDate);

        public Task<decimal> GetExchangeRateLocalAsync(short CompanyId, short UserId, Int16 CurrencyId, string TrnsDate);

        public Task<bool> GetCheckPeriodClosedAsync(short CompanyId, short UserId, Int16 ModuleId, string TrnsDate);

        public Task<decimal> GetGstPercentageAsync(short CompanyId, short UserId, Int16 GstId, string TrnsDate);

        public Task<decimal> GetCreditTermDayAsync(short CompanyId, short UserId, Int16 CreditTermId, string TrnsDate);

        public Task<DocSeqNoViewModel> GetDocSeqNoByTransactionAsync(short CompanyId, short UserId, Int16 ModuleId, Int16 TransactionId);

        public Task<DynamicLookupViewModel> GetDynamicLookupAsync(short CompanyId, short UserId);

        public Task<SqlResponse> SaveDynamicLookupAsync(short CompanyId, short UserId, S_DynamicLookup s_DynamicLookup);
    }
}