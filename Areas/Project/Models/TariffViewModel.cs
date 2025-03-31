namespace AEMSWEB.Areas.Project.Models
{
    public class SaveTariffViewModel
    {
        public TariffViewModel tariff { get; set; }
        public string? companyId { get; set; }
    }

    public class TariffViewModel
    {
        public byte CompanyId { get; set; } = 1; // Default value
        public long TariffId { get; set; }
        public string? RateType { get; set; } // Nullable
        public short? TaskId { get; set; }
        public string? TaskCode { get; set; }
        public string? TaskName { get; set; }
        public short? ChargeId { get; set; }
        public string? ChargeCode { get; set; }
        public string? ChargeName { get; set; }
        public int? PortId { get; set; }
        public string? PortCode { get; set; }
        public string? PortName { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public short CurrencyId { get; set; } = 0;
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }
        public short? UomId { get; set; }
        public string? UomCode { get; set; }
        public string? UomName { get; set; }
        public int? SlabUomId { get; set; }
        public string? SlabUomCode { get; set; }
        public string? SlabUomName { get; set; }
        public short? VisaTypeId { get; set; }
        public string? VisaTypeCode { get; set; }
        public string? VisaTypeName { get; set; }
        public short? FromPlace { get; set; }
        public string? FromPlaceName { get; set; }
        public short? ToPlace { get; set; }
        public string? ToPlaceName { get; set; }
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
        public string? Remarks { get; set; } // Nullable
        public bool IsActive { get; set; } = true; // Default value
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default value
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class TariffViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<TariffViewModel> data { get; set; }
    }

    public class TaskCountViewModel
    {
        public int? TaskId { get; set; }
        public int CountId { get; set; }
    }

    public class TaskCountsViewModel
    {
        public int PortExpense { get; set; }
        public int LaunchServices { get; set; }
        public int EquipmentsUsed { get; set; }
        public int CrewSignOn { get; set; }
        public int CrewSignOff { get; set; }
        public int CrewMiscellaneous { get; set; }
        public int MedicalAssistance { get; set; }
        public int ConsignmentImport { get; set; }
        public int ConsignmentExport { get; set; }
        public int ThirdPartySupply { get; set; }
        public int FreshWaterSupply { get; set; }
        public int TechniciansSurveyors { get; set; }
        public int LandingItems { get; set; }
        public int OtherService { get; set; }
        public int AgencyRemuneration { get; set; }
        public int Visa { get; set; }
    }
}