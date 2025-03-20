namespace AEMSWEB.Entities.Accounts.AR
{
    public class ArOutstandTransactions
    {
        public Int16 CompanyId { get; set; }
        public Int16 TransactionId { get; set; }
        public Int64 DocumentId { get; set; }
        public string? DocumentNo { get; set; }
        public string? ReferenceNo { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime DueDate { get; set; }
        public Int32 CustomerId { get; set; }
        public Int16 CurrencyId { get; set; }
        public decimal ExhRate { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal BalAmt { get; set; }
        public decimal BalLocalAmt { get; set; }
        public string? Remarks { get; set; }
        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
    }
}