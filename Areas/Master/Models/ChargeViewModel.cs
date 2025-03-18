namespace AEMSWEB.Models.Masters
{
    public class ChargeViewModel
    {
        public Int32 ChargeId { get; set; }
        public string ChargeCode { get; set; }
        public string ChargeName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveChargeViewModel
    {
        public ChargeViewModel Charge { get; set; }
        public string CompanyId { get; set; }
    }

    public class ChargeViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<ChargeViewModel> data { get; set; }
    }
}