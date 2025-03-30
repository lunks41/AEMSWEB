using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Project
{
    public class Ser_Tariff
    {
        public Int16 CompanyId { get; set; }
        [Key]
        public long TariffId { get; set; }
        public string? RateType { get; set; }
        public short? TaskId { get; set; }
        public short? ChargeId { get; set; }
        public int? PortId { get; set; }
        public int? CustomerId { get; set; }
        public short CurrencyId { get; set; } = 0;
        public short? UomId { get; set; }
        public int? SlabUomId { get; set; }
        public short? VisaTypeId { get; set; }
        public short? FromPlace { get; set; }
        public short? ToPlace { get; set; }
        public decimal? DisplayRate { get; set; }
        public decimal? BasicRate { get; set; }
        public decimal? MinUnit { get; set; }
        public decimal? MaxUnit { get; set; }
        public bool IsAdditional { get; set; } = false;
        public decimal? AdditionalRate { get; set; }
        public decimal? AdditionalUnit { get; set; }
        public bool IsPrepayment { get; set; } = false;
        public decimal? PrepaymentPercentage { get; set; }
        public int ItemNo { get; set; } = 0;
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}