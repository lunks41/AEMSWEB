namespace AEMSWEB.Entities.Project
{
    public class Ser_Tariff
    {
        public short CompanyId { get; set; } = 1; // Default value
        public long TariffId { get; set; }
        public string? RateType { get; set; } // Nullable
        public short? TaskId { get; set; }
        public short? ChargeId { get; set; }
        public int? PortId { get; set; }
        public int? CustomerId { get; set; }
        public short CurrencyId { get; set; } = 0; // Default value
        public short? UomId { get; set; }
        public int? SlabUomId { get; set; }
        public short? VisaTypeId { get; set; }
        public short? FromPlace { get; set; }
        public short? ToPlace { get; set; }
        public decimal? DisplayRate { get; set; }
        public decimal? BasicRate { get; set; }
        public decimal? MinUnit { get; set; }
        public decimal? MaxUnit { get; set; }
        public bool IsAdditional { get; set; } = false; // Default value
        public decimal? AdditionalUnit { get; set; }
        public decimal? AdditionalRate { get; set; }
        public decimal? PrepaymentPercentage { get; set; }
        public bool IsPrepayment { get; set; } = false; // Default value
        public int ItemNo { get; set; } = 0; // Default value
        public string Remarks { get; set; } // Nullable
        public bool IsActive { get; set; } = true; // Default value
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default value
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}