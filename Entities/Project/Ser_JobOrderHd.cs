namespace AMESWEB.Entities.Project
{
    public class Ser_JobOrderHd
    {
        public byte? CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public int? CustomerId { get; set; }
        public short? CurrencyId { get; set; }
        public decimal? ExhRate { get; set; }
        public Int32 VesselId { get; set; }
        public string IMONo { get; set; }
        public byte? VesselDistance { get; set; }
        public short? PortId { get; set; }
        public short? LastPortId { get; set; }
        public short? NextPortId { get; set; }
        public short? VoyageId { get; set; }
        public short? StatusId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsTaxable { get; set; }
        public bool? IsClose { get; set; }
        public bool? IsPost { get; set; }
        public byte? GSTId { get; set; }
        public DateTime? EtaDate { get; set; }
        public DateTime? EtdDate { get; set; }
        public long? InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? SeriesDate { get; set; }
        public int? AddressId { get; set; }
        public int? ContactId { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? TotalLocalAmt { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAgent { get; set; }
        public string Remarks { get; set; }
        public string EditVersion { get; set; }
    }
}
