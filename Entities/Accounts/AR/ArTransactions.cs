namespace AEMSWEB.Entities.Accounts.AR
{
    public class ArTransactions
    {
        public Int16 CompanyId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public Int64 DocumentId { get; set; }
        public string? DocumentNo { get; set; }
        public string? ReferenceNo { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime DueDate { get; set; }
        public Int32 CustomerId { get; set; }
        public Int16 CurrencyId { get; set; }
        public Int16 RefModuleId { get; set; }
        public Int16 RefTransactionId { get; set; }
        public Int64 RefDocumentId { get; set; }
        public string? RefDocumentNo { get; set; }
        public string? RefReferenceNo { get; set; }
        public DateTime RefAccountDate { get; set; }
        public Int32 RefCustomerId { get; set; }
        public Int16 RefCurrencyId { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal AllAmt { get; set; }
        public decimal AllLocalAmt { get; set; }
        public decimal ExGainLoss { get; set; }
    }
}