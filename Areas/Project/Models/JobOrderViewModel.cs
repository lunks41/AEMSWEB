using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Areas.Project.Models
{
    public class SaveJobOrderHdViewModel
    {
        public JobOrderHdViewModel jobOrderHd { get; set; }
        public string? companyId { get; set; }
    }

    public class SaveJobOrderDtViewModel
    {
        public JobOrderDtViewModel jobOrderDt { get; set; }
        public string? companyId { get; set; }
    }

    public class JobOrderHdViewModel
    {
        public long JobOrderId { get; set; }
        public byte? CompanyId { get; set; }
        public string? JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public short? CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }
        public decimal? ExhRate { get; set; }
        public Int32 VesselId { get; set; }
        public string? VesselName { get; set; }
        public string? IMONo { get; set; }
        public byte? VesselDistance { get; set; }
        public short? PortId { get; set; }
        public string? PortName { get; set; }
        public short? LastPortId { get; set; }
        public string? LastPortName { get; set; }
        public short? NextPortId { get; set; }
        public string? NextPortName { get; set; }
        public short? VoyageId { get; set; }
        public string? VoyageNo { get; set; }
        public string? NatureOfCall { get; set; }
        public string? ISPS { get; set; }
        public DateTime? EtaDate { get; set; }
        public DateTime? EtdDate { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerAgent { get; set; }
        public string? MasterName { get; set; }
        public string? Charters { get; set; }
        public string? ChartersAgent { get; set; }
        public long? InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? SeriesDate { get; set; }
        public int? AddressId { get; set; }
        public int? ContactId { get; set; }
        public string? Remark1 { get; set; }
        public string? Remark2 { get; set; }
        public short? StatusId { get; set; }
        public string? StatusName { get; set; }
        public byte? GSTId { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? TotalLocalAmt { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTaxable { get; set; }
        public bool? IsClose { get; set; }
        public bool? IsPost { get; set; }
        public string? EditVersion { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class JobOrderDtViewModel
    {
        [Key]
        public long JobOrderId { get; set; }

        public byte? CompanyId { get; set; }
        public string? JobOrderNo { get; set; }

        [Key]
        public Int16 ItemNo { get; set; }

        public Int16 TaskId { get; set; }
        public Int16 TaskItemNo { get; set; }
        public Int64 ServiceId { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal GstAmt { get; set; }
        public decimal GstLocalAmt { get; set; }
        public decimal TotAftAmt { get; set; }
        public decimal TotLocalAftAmt { get; set; }
    }

    public class JobOrderViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<JobOrderHdViewModel> data { get; set; }
    }

    public class StatusCountViewModel
    {
        public int? StatusId { get; set; }
        public int CountId { get; set; }
    }

    public class StatusCountsViewModel
    {
        public int All { get; set; }
        public int Pending { get; set; }
        public int Confirm { get; set; }
        public int Completed { get; set; }
        public int Cancel { get; set; }
        public int Post { get; set; }
        public int CancelWithService { get; set; }
    }
}