using AMESWEB.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Models
{
    public class HeaderViewModelV1
    {
        private string _searchStr;
        private DateTime _fromdate;
        private DateTime _todate;

        [FromHeader]
        public Int16 CompanyId { get; set; }

        [FromHeader]
        public Int16 UserId { get; set; }

        [FromHeader]
        public Int16 pageSize { get; set; }

        [FromHeader]
        public Int16 pageNumber { get; set; }

        [FromHeader]
        public string? searchString
        { get { return _searchStr == null ? string.Empty : _searchStr.Trim(); } set { _searchStr = value; } }

        [FromHeader]
        public Int16 ModuleId { get; set; }

        [FromHeader]
        public Int16 TransactionId { get; set; }

        [FromHeader]
        public string? DocumentId { get; set; }

        [FromHeader]
        public string? fromDate
        {
            get { return DateHelperStatic.FormatDate(_fromdate); }
            set { _fromdate = DateHelperStatic.ParseDBDate(value); }
        }

        [FromHeader]
        public string? toDate
        {
            get { return DateHelperStatic.FormatDate(_todate); }
            set { _todate = DateHelperStatic.ParseDBDate(value); }
        }
    }

    public class HeaderViewModel
    {
        private string _searchStr;

        [FromHeader]
        public Int16 CompanyId { get; set; }

        [FromHeader]
        public Int16 UserId { get; set; }

        [FromHeader]
        public Int16 pageSize { get; set; }

        [FromHeader]
        public Int16 pageNumber { get; set; }

        [FromHeader]
        public string? searchString
        { get { return _searchStr == null ? string.Empty : _searchStr.Trim(); } set { _searchStr = value; } }
    }
}