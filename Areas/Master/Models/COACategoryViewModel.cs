namespace AEMSWEB.Models.Masters
{
    public class COACategoryViewModel
    {
        public Int16 COACategoryId { get; set; }
        public Int16 CompanyId { get; set; }
        public string COACategoryCode { get; set; }
        public string COACategoryName { get; set; }
        public Int16 SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}