namespace AMESWEB.Areas.Account.Models
{
    public class GetOutstandTransactionViewModel
    {
        public bool Sel { get; set; }
        public string? DocumentId { get; set; }
        public string? DocumentNo { get; set; }
        public string? ReferenceNo { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime DueDate { get; set; }
        public byte CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }
        public decimal ExhRate { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal BalAmt { get; set; }
        public decimal BalLocalAmt { get; set; }
        public short ModuleId { get; set; }
        public short TransactionId { get; set; }
        public string? TrnType { get; set; }
    }
}