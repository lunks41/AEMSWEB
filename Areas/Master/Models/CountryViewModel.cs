﻿namespace AMESWEB.Models.Masters
{
    public class CountryViewModel
    {
        public Int16 CountryId { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public Int16 CompanyId { get; set; }
        public string? ISD { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveCountryViewModel
    {
        public CountryViewModel country { get; set; }
        public string? companyId { get; set; }
    }

    public class CountryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CountryViewModel> data { get; set; }
    }
}