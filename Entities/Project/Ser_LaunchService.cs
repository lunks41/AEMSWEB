using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_LaunchService
    {
        public byte CompanyId { get; set; }

        [Key]
        public long LaunchServiceId { get; set; }

        public DateTime LaunchServiceDate { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } // Not nullable
        public short TaskId { get; set; } = 2; // Default value
        public short UomId { get; set; }
        public short GLId { get; set; }
        public short ChargeId { get; set; }
        public string AmeTally { get; set; } // Nullable
        public string BoatopTally { get; set; } // Nullable
        public decimal? DistanceFromJetty { get; set; } // Nullable
        public DateTime? LoadingTime { get; set; } // Nullable
        public DateTime? LeftJetty { get; set; } // Nullable
        public DateTime? AlongsideVessel { get; set; } // Nullable
        public DateTime? DepartedFromVessel { get; set; } // Nullable
        public DateTime? ArrivedAtJetty { get; set; } // Nullable
        public decimal? LaunchWaitingTime { get; set; } // Nullable
        public decimal? TimeDiff { get; set; } // Nullable
        public decimal DistanceFromJettyToVessel { get; set; } = 0.00m; // Default value
        public decimal WeightOfCargoDelivered { get; set; } = 0.00m; // Default value
        public decimal WeightOfCargoLanded { get; set; } = 0.00m; // Default value
        public string BoatOperator { get; set; } // Nullable
        public string Annexure { get; set; } // Nullable
        public string InvoiceNo { get; set; } // Nullable
        public int? PortId { get; set; } // Nullable
        public short? BargeId { get; set; } // Nullable
        public string BargeName { get; set; } // Nullable
        public short StatusId { get; set; }
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
    }
}