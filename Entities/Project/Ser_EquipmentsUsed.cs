using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_EquipmentsUsed
    {
        [Key]
        public long EquipmentsUsedId { get; set; }

        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } // Nullable
        public short TaskId { get; set; }
        public long? DebitNoteId { get; set; } // Nullable
        public string DebitNoteNo { get; set; } // Nullable
        public decimal TotAmt { get; set; } = 0m;
        public decimal GstAmt { get; set; } = 0m;
        public decimal TotAmtAftGst { get; set; } = 0m;
        public string Remarks { get; set; } = string.Empty; // Default value
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default current date
        public short? EditById { get; set; } // Nullable
        public DateTime? EditDate { get; set; } // Nullable
        public byte EditVersion { get; set; }
        public decimal Quantity { get; set; } = 0m; // Default value
        public short GLId { get; set; }
        public DateTime EquipmentsTimeSheetDate { get; set; }
        public short? ChargeId { get; set; } // Nullable
        public string MorningTimeIn { get; set; } // Nullable
        public string MorningTimeOut { get; set; } // Nullable
        public string MorningTotalHours { get; set; } // Nullable
        public string EveningTimeIn { get; set; } // Nullable
        public string EveningTimeOut { get; set; } // Nullable
        public string EveningTotalHours { get; set; } // Nullable
        public string TotalRegularHours { get; set; } // Nullable
        public string TotalOvertimeHours { get; set; } // Nullable
        public int? Crane { get; set; } = 0; // Default value
        public int? Forklift { get; set; } = 0; // Default value
        public string Labour { get; set; } // Not nullable
        public string Mafi { get; set; } // Not nullable
        public DateTime EquipmentsTimeSheetDateInGmt { get; set; }
        public string ReferenceNo { get; set; } // Not nullable
        public short? UomId { get; set; } // Nullable
        public short StatusId { get; set; }
        public int? Stevedor { get; set; } = 0; // Default value
        public int? LaunchId { get; set; } // Nullable
        public string LaunchServiceDebitNoteNo { get; set; } // Nullable
        public short ForkliftChargeId { get; set; } = 0; // Default value
        public short CraneChargeId { get; set; } = 0; // Default value
        public short StevedorChargeId { get; set; } = 0; // Default value
        public string LoadingRefNo { get; set; } = string.Empty; // Default value
        public string OffloadingRefNo { get; set; } = string.Empty; // Default value
        public int CraneOffloading { get; set; } = 0; // Default value
        public int ForkliftOffloading { get; set; } = 0; // Default value
        public int StevedorOffloading { get; set; } = 0; // Default value
        public bool? IsEquimentFooter { get; set; } // Nullable
        public string EquimentFooter { get; set; } // Nullable
    }
}