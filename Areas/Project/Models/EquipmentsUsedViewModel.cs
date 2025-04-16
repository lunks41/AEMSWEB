namespace AMESWEB.Areas.Project.Models
{
    public class SaveEquipmentsUsedViewModel
    {
        public EquipmentsUsedViewModel equipmentsUsed { get; set; }
        public string? companyId { get; set; }
    }

    public class EquipmentsUsedViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<EquipmentsUsedViewModel> data { get; set; }
    }

    public class EquipmentsUsedViewModel
    {
        public long EquipmentsUsedId { get; set; }
        public DateTime Date { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } = string.Empty;
        public short TaskId { get; set; }
        public decimal Quantity { get; set; } = 0m;
        public short ChargeId { get; set; }
        public string? ChargeName { get; set; } = string.Empty;
        public short UomId { get; set; }
        public string? UomName { get; set; } = string.Empty;
        public short GLId { get; set; }
        public string? GlName { get; set; } = string.Empty;
        public string? Mafi { get; set; }
        public string? Others { get; set; }
        public short? ForkliftChargeId { get; set; } = 0;
        public string? ForkliftChargeName { get; set; } = string.Empty;
        public short? CraneChargeId { get; set; } = 0;
        public string? CraneChargeName { get; set; } = string.Empty;
        public short? StevedorChargeId { get; set; } = 0;
        public string? StevedorChargeName { get; set; } = string.Empty;
        public string? LoadingRefNo { get; set; } = string.Empty;
        public byte? Craneloading { get; set; } = 0;
        public byte? Forkliftloading { get; set; } = 0;
        public byte? Stevedorloading { get; set; } = 0;
        public string? OffloadingRefNo { get; set; } = string.Empty;
        public byte? CraneOffloading { get; set; } = 0;
        public byte? ForkliftOffloading { get; set; } = 0;
        public byte? StevedorOffloading { get; set; } = 0;
        public string? MorningTimeIn { get; set; }
        public string? MorningTimeOut { get; set; }
        public string? MorningTotalHours { get; set; }
        public string? EveningTimeIn { get; set; }
        public string? EveningTimeOut { get; set; }
        public string? EveningTotalHours { get; set; }
        public string? TotalRegularHours { get; set; }
        public string? TotalOvertimeHours { get; set; }
        public string? DriverName { get; set; }
        public string? VehicleName { get; set; }
        public long? LaunchServiceId { get; set; }
        public string? LaunchServiceDebitNoteNo { get; set; }
        public string? Remarks { get; set; } = string.Empty;
        public short StatusId { get; set; }
        public string? StatusName { get; set; } = string.Empty;
        public bool? IsEquimentFooter { get; set; } = false;
        public string? EquimentFooter { get; set; }
        public long? DebitNoteId { get; set; }
        public string? DebitNoteNo { get; set; }
        public decimal TotAmt { get; set; } = 0m;
        public decimal GstAmt { get; set; } = 0m;
        public decimal TotAmtAftGst { get; set; } = 0m;
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; } = string.Empty;
        public string? EditBy { get; set; } = string.Empty;
        public byte EditVersion { get; set; }
    }
}