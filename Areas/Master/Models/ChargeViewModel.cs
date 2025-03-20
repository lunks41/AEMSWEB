namespace AEMSWEB.Models.Masters
{
    public class ChargesViewModel
    {
        public Int32 ChargeId { get; set; }
        public string? ChargeCode { get; set; }
        public string? ChargeName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveChargesViewModel
    {
        public ChargesViewModel charges { get; set; }
        public string? companyId { get; set; }
    }

    public class ChargesViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<ChargesViewModel> data { get; set; }
    }
}