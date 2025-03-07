namespace AEMSWEB.Models.Masters
{
    public class GstViewModel
    {
        public Int16 GstId { get; set; }
        public Int16 CompanyId { get; set; }
        public Int16 GstCategoryId { get; set; }
        public string GstCategoryCode { get; set; }
        public string GstCategoryName { get; set; }
        public string GstCode { get; set; }
        public string GstName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveGstViewModel
    {
        public GstViewModel Gst { get; set; }
        public string CompanyId { get; set; }
    }
}