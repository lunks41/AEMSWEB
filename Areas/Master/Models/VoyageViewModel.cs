﻿namespace AMESWEB.Models.Masters
{
    public class VoyageViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 VoyageId { get; set; }
        public string? VoyageNo { get; set; }
        public string? ReferenceNo { get; set; }
        public Int32 VesselId { get; set; }
        public string? VesselCode { get; set; }
        public string? VesselName { get; set; }
        public Int16 BargeId { get; set; }
        public string? BargeCode { get; set; }
        public string? BargeName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveVoyageViewModel
    {
        public VoyageViewModel voyage { get; set; }
        public string? companyId { get; set; }
    }

    public class VoyageViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<VoyageViewModel> data { get; set; }
    }
}