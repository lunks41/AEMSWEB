using AMESWEB.Areas.Setting.Models;
using AMESWEB.Entities.Setting;
using AMESWEB.Models;

namespace AMESWEB.Areas.Setting.Data
{
    public interface ISettingService
    {
        #region Decimal Setting

        public Task<DecimalSettingViewModel> GetDecSettingAsync(short CompanyId, short UserId);

        public Task<SqlResponce> SaveDecSettingAsync(short CompanyId, short UserId, S_DecSettings s_DecSettings);

        #endregion Decimal Setting

        #region Finance Setting

        public Task<FinanceSettingViewModel> GetFinSettingAsync(short CompanyId, short UserId);

        public Task<SqlResponce> SaveFinSettingAsync(short CompanyId, short UserId, S_FinSettings s_FinSettings);

        #endregion Finance Setting

        #region Task Setting

        public Task<TaskSettingViewModel> GetTaskSettingAsync(short CompanyId, short UserId);

        public Task<SqlResponce> SaveTaskSettingAsync(short CompanyId, short UserId, S_TaskSettings s_TaskSettings);

        #endregion Task Setting

        #region Mandatory Fields

        public Task<IEnumerable<MandatoryFieldsViewModel>> GetMandatoryFieldsByIdAsync(short CompanyId, short UserId, Int16 ModuleId);

        public Task<SqlResponce> SaveMandatoryFieldsAsync(short CompanyId, short UserId, List<S_MandatoryFields> s_MandatoryFields);

        #endregion Mandatory Fields

        #region Visible Fields

        public Task<VisibleFieldsViewModel> GetVisibleFieldsByIdAsync(short CompanyId, short UserId, Int16 ModuleId, Int16 TransactionId);

        public Task<IEnumerable<VisibleFieldsViewModel>> GetVisibleFieldsByIdAsync(short CompanyId, short UserId, Int16 ModuleId);

        public Task<SqlResponce> SaveVisibleFieldsAsync(short CompanyId, short UserId, List<S_VisibleFields> s_VisibleFields);

        #endregion Visible Fields

        #region Number Format

        public Task<ModelNameViewModelCount> GetNumberFormatListAsync(short CompanyId, short UserId);

        public Task<NumberSettingViewModel> GetNumberFormatByIdAsync(short CompanyId, short UserId, Int32 ModuleId, Int32 TransactionId);

        public Task<NumberSettingDtViewModel> GetNumberFormatByYearAsync(short CompanyId, short UserId, Int32 NumberId, Int32 NumYear);

        public Task<SqlResponce> SaveNumberFormatAsync(short CompanyId, short UserId, S_NumberFormat s_NumberFormat);

        #endregion Number Format

        #region User Grid

        public Task<UserGridViewModelCount> GetUserGridAsync(short CompanyId, short UserId, Int16 ModuleId, Int16 TransactionId);

        public Task<UserGridViewModel> GetUserGridByIdAsync(short CompanyId, short UserId, UserGridViewModel userGridViewModel);

        public Task<IEnumerable<UserGridViewModel>> GetUserGridByUserIdAsync(short CompanyId, short UserId, Int16 SelectedUserId);

        public Task<SqlResponce> SaveUserGridAsync(short CompanyId, short UserId, S_UserGrdFormat s_UserGrdFormat);

        public Task<SqlResponce> CloneUserGridSettingAsync(short CompanyId, short UserId, Int16 FromUserId, Int16 ToUserId);

        public Task<UserSettingViewModel> GetUserSettingAsync(short CompanyId, short UserId);

        public Task<SqlResponce> SaveUserSettingAsync(short CompanyId, short UserId, S_UserSettings S_UserSettings);

        #endregion User Grid

        public Task<decimal> GetExchangeRateAsync(short CompanyId, short UserId, Int16 CurrencyId, DateTime? TrnsDate);

        public Task<decimal> GetExchangeRateLocalAsync(short CompanyId, short UserId, Int16 CurrencyId, string TrnsDate);

        public Task<bool> GetCheckPeriodClosedAsync(short CompanyId, short UserId, Int16 ModuleId, string TrnsDate);

        public Task<decimal> GetGstPercentageAsync(short CompanyId, short UserId, Int16 GstId, string TrnsDate);

        public Task<decimal> GetCreditTermDayAsync(short CompanyId, short UserId, Int16 CreditTermId, string TrnsDate);

        public Task<DocSeqNoViewModel> GetDocSeqNoByTransactionAsync(short CompanyId, short UserId, Int16 ModuleId, Int16 TransactionId);

        public Task<DynamicLookupViewModel> GetDynamicLookupAsync(short CompanyId, short UserId);

        public Task<SqlResponce> SaveDynamicLookupAsync(short CompanyId, short UserId, S_DynamicLookup s_DynamicLookup);
    }
}