using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_LaunchServices
    {
        public long LaunchServiceId { get; set; }
        public DateTime LaunchServiceDate { get; set; }
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; }
        public short TaskId { get; set; } = 2; // Default value
        public short GLId { get; set; }
        public short ChargeId { get; set; }
        public short UomId { get; set; }
        public string AmeTally { get; set; }
        public string BoatopTally { get; set; }
        public decimal? DistanceFromJetty { get; set; }
        public DateTime? LoadingTime { get; set; }
        public DateTime? LeftJetty { get; set; }
        public DateTime? AlongsideVessel { get; set; }
        public DateTime? DepartedFromVessel { get; set; }
        public DateTime? ArrivedAtJetty { get; set; }
        public decimal? LaunchWaitingTime { get; set; }
        public decimal? TimeDiff { get; set; }
        public decimal DistanceFromJettyToVessel { get; set; } = 0.00m; // Default value
        public decimal WeightOfCargoDelivered { get; set; } = 0.00m; // Default value
        public decimal WeightOfCargoLanded { get; set; } = 0.00m; // Default value
        public string BoatOperator { get; set; }
        public string Annexure { get; set; }
        public string InvoiceNo { get; set; }
        public short? PortId { get; set; }
        public short? BargeId { get; set; }
        public string? BargeName { get; set; }
        public short StatusId { get; set; }
        public long? DebitNoteId { get; set; }
        public string DebitNoteNo { get; set; }
        public decimal TotAmt { get; set; } = 0.00m; // Default value
        public decimal GstAmt { get; set; } = 0.00m; // Default value
        public decimal TotAmtAftGst { get; set; } = 0.00m; // Default value
        public string Remarks { get; set; } = string.Empty; // Default value
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default value
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public byte EditVersion { get; set; }
    }
}