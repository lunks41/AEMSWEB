namespace AEMSWEB.Entities.Project
{
    public class Ser_JobOrderDt

    {
        public byte CompanyId { get; set; } = 0;
        public long JobOrderId { get; set; } = 0;
        public string JobOrderNo { get; set; } = "(\"\")";
        public byte ItemNo { get; set; } = 0;
        public byte TaskId { get; set; } = 0;
        public byte TaskItemNo { get; set; } = 0;
        public long ServiceId { get; set; } = 0;
        public decimal TotAmt { get; set; } = 0;
        public decimal TotLocalAmt { get; set; } = 0;
        public decimal GstAmt { get; set; } = 0;
        public decimal GstLocalAmt { get; set; } = 0;
        public decimal TotAftAmt { get; set; } = 0;
        public decimal TotLocalAftAmt { get; set; } = 0;
    }
}