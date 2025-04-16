using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_EquipmentsUsed
    {
        [Key]
        public long EquipmentsUsedId { get; set; } // bigint
        public DateTime Date { get; set; } // date
        public string ReferenceNo { get; set; } = string.Empty; // nvarchar(20)
        public byte CompanyId { get; set; } // tinyint
        public long JobOrderId { get; set; } // bigint
        public string JobOrderNo { get; set; } = string.Empty; // varchar(50)
        public short TaskId { get; set; } // smallint
        public decimal Quantity { get; set; } = 0m; // decimal(18,2)
        public short ChargeId { get; set; } // smallint
        public short UomId { get; set; } // smallint
        public short GLId { get; set; } // smallint
        public string? Mafi { get; set; } // varchar(50) nullable
        public string? Others { get; set; } // varchar(50) nullable
        public short? ForkliftChargeId { get; set; } = 0; // smallint nullable
        public short? CraneChargeId { get; set; } = 0; // smallint nullable
        public short? StevedorChargeId { get; set; } = 0; // smallint nullable
        public string? LoadingRefNo { get; set; } = string.Empty; // varchar(50) nullable
        public byte? Craneloading { get; set; } = 0; // tinyint nullable
        public byte? Forkliftloading { get; set; } = 0; // tinyint nullable
        public byte? Stevedorloading { get; set; } = 0; // tinyint nullable
        public string? OffloadingRefNo { get; set; } = string.Empty; // varchar(50) nullable
        public byte? CraneOffloading { get; set; } = 0; // tinyint nullable
        public byte? ForkliftOffloading { get; set; } = 0; // tinyint nullable
        public byte? StevedorOffloading { get; set; } = 0; // tinyint nullable
        public string? MorningTimeIn { get; set; } // varchar(8) nullable
        public string? MorningTimeOut { get; set; } // varchar(8) nullable
        public string? MorningTotalHours { get; set; } // varchar(8) nullable
        public string? EveningTimeIn { get; set; } // varchar(8) nullable
        public string? EveningTimeOut { get; set; } // varchar(8) nullable
        public string? EveningTotalHours { get; set; } // varchar(8) nullable
        public string? TotalRegularHours { get; set; } // varchar(8) nullable
        public string? TotalOvertimeHours { get; set; } // varchar(8) nullable
        public string? DriverName { get; set; } // varchar(50) nullable
        public string? VehicleName { get; set; } // varchar(50) nullable
        public long? LaunchServiceId { get; set; } // bigint nullable
        public string? LaunchServiceDebitNoteNo { get; set; } // varchar(50) nullable
        public string? Remarks { get; set; } = string.Empty; // varchar(500) nullable
        public short StatusId { get; set; } // smallint
        public bool? IsEquimentFooter { get; set; } = false; // bit nullable
        public string? EquimentFooter { get; set; } // varchar(100) nullable
        public long? DebitNoteId { get; set; } // bigint nullable
        public string? DebitNoteNo { get; set; } // varchar(50) nullable
        public decimal TotAmt { get; set; } = 0m; // decimal(18,4)
        public decimal GstAmt { get; set; } = 0m; // decimal(18,4)
        public decimal TotAmtAftGst { get; set; } = 0m; // decimal(18,4)
        public short CreateById { get; set; } // smallint
        public DateTime CreateDate { get; set; } = DateTime.Now; // datetime
        public short? EditById { get; set; } // smallint nullable
        public DateTime? EditDate { get; set; } // datetime nullable
        public byte EditVersion { get; set; } // tinyint
    }
}