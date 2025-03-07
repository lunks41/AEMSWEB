using AEMSWEB.Helper;

namespace AEMSWEB.Areas.Account.Models.GL
{
    public class GLPeriodCloseViewModel
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private DateTime? _arCloseDate;
        private DateTime? _apCloseDate;
        private DateTime? _cbCloseDate;
        private DateTime? _glCloseDate;
        public short CompanyId { get; set; }
        public int FinYear { get; set; }
        public short FinMonth { get; set; }

        public string StartDate
        {
            get { return DateHelperStatic.FormatDate(_startDate); }
            set { _startDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string EndDate
        {
            get { return DateHelperStatic.FormatDate(_endDate); }
            set { _endDate = DateHelperStatic.ParseDBDate(value); }
        }

        public bool IsArClose { get; set; }
        public int ArCloseById { get; set; }

        public string ArCloseDate
        {
            get { return _arCloseDate.HasValue ? DateHelperStatic.FormatDate(_arCloseDate.Value) : ""; }
            set { _arCloseDate = string.IsNullOrEmpty(value) ? null : DateHelperStatic.ParseDBDate(value); }
        }

        //get { return DateHelperStatic.FormatDate(_arCloseDate); }
        //set { _arCloseDate = DateHelperStatic.ParseDBDate(value); }

        public bool IsApClose { get; set; }
        public int ApCloseById { get; set; }

        public string ApCloseDate
        {
            get { return _apCloseDate.HasValue ? DateHelperStatic.FormatDate(_apCloseDate.Value) : ""; }
            set { _apCloseDate = string.IsNullOrEmpty(value) ? null : DateHelperStatic.ParseDBDate(value); }
        }

        public bool IsCbClose { get; set; }
        public int CbCloseById { get; set; }

        public string CbCloseDate
        {
            get { return _cbCloseDate.HasValue ? DateHelperStatic.FormatDate(_cbCloseDate.Value) : ""; }
            set { _cbCloseDate = string.IsNullOrEmpty(value) ? null : DateHelperStatic.ParseDBDate(value); }
        }

        public bool IsGlClose { get; set; }
        public int GlCloseById { get; set; }

        public string GlCloseDate
        {
            get { return _glCloseDate.HasValue ? DateHelperStatic.FormatDate(_glCloseDate.Value) : ""; }
            set { _glCloseDate = string.IsNullOrEmpty(value) ? null : DateHelperStatic.ParseDBDate(value); }
        }

        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
        public string ArCloseBy { get; set; }
        public string ApCloseBy { get; set; }
        public string CbCloseBy { get; set; }
        public string GlCloseBy { get; set; }
    }

    public class PeriodCloseViewModel
    {
        public int FinYear { get; set; }
        public short FinMonth { get; set; }
        public string FieldName { get; set; }
        public bool IsValue { get; set; }
    }
}