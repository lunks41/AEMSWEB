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
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string? JobOrderNo { get; set; }
        public short TaskId { get; set; }
        public long? DebitNoteId { get; set; }
        public string? DebitNoteNo { get; set; }
        public decimal TotAmt { get; set; } = 0.00m;
        public decimal GstAmt { get; set; } = 0.00m;
        public decimal TotAmtAftGst { get; set; } = 0.00m;
        public decimal Quantity { get; set; } = 0.00m;
        public short GLId { get; set; }
        public DateTime EquipmentsTimeSheetDate { get; set; }
        public short? ChargeId { get; set; }
        public string? MorningTimeIn { get; set; }
        public string? MorningTimeOut { get; set; }
        public string? MorningTotalHours { get; set; }
        public string? EveningTimeIn { get; set; }
        public string? EveningTimeOut { get; set; }
        public string? EveningTotalHours { get; set; }
        public string? TotalRegularHours { get; set; }
        public string? TotalOvertimeHours { get; set; }
        public int? Crane { get; set; } = 0;
        public int? Forklift { get; set; } = 0;
        public string? Labour { get; set; }
        public string? Mafi { get; set; }
        public DateTime EquipmentsTimeSheetDateInGmt { get; set; }
        public string? ReferenceNo { get; set; }
        public short? UomId { get; set; }
        public short StatusId { get; set; }
        public int? Stevedor { get; set; } = 0;
        public int? LaunchId { get; set; }
        public string? LaunchServiceDebitNoteNo { get; set; }
        public short ForkliftChargeId { get; set; } = 0;
        public short CraneChargeId { get; set; } = 0;
        public short StevedorChargeId { get; set; } = 0;
        public string? LoadingRefNo { get; set; } = string.Empty;
        public string? OffloadingRefNo { get; set; } = string.Empty;
        public int CraneOffloading { get; set; } = 0;
        public int ForkliftOffloading { get; set; } = 0;
        public int StevedorOffloading { get; set; } = 0;
        public bool? IsEquimentFooter { get; set; }
        public string? EquimentFooter { get; set; }
        public string? Remarks { get; set; } = string.Empty;
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public byte EditVersion { get; set; }
    }
}