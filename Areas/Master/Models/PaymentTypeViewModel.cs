namespace AEMSWEB.Models.Masters
{
    public class PaymentTypeViewModel
    {
        public Int16 PaymentTypeId { get; set; }
        public Int16 CompanyId { get; set; }
        public string PaymentTypeCode { get; set; }
        public string PaymentTypeName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SavePaymentTypeViewModel
    {
        public PaymentTypeViewModel PaymentType { get; set; }
        public string CompanyId { get; set; }
    }
}