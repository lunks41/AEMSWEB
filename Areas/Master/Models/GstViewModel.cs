using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Models.Masters
{
    public class GstViewModel
    {
        public Int16 GstId { get; set; }
        public Int16 CompanyId { get; set; }
        public Int16 GstCategoryId { get; set; }
        public string? GstCategoryCode { get; set; }
        public string? GstCategoryName { get; set; }
        public string? GstCode { get; set; }
        public string? GstName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveGstViewModel
    {
        public GstViewModel gst { get; set; }
        public string? companyId { get; set; }
    }

    public class GstDtViewModel
    {
        public Int16 GstId { get; set; }
        public string? GstCode { get; set; }
        public string? GstName { get; set; }
        public Int16 CompanyId { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal GstPercentage { get; set; }

        public DateTime ValidFrom { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveGstDtViewModel
    {
        public GstDtViewModel gstDt { get; set; }
        public string? companyId { get; set; }
    }

    public class GstCategoryViewModel
    {
        public Int16 GstCategoryId { get; set; }
        public Int16 CompanyId { get; set; }
        public string? GstCategoryCode { get; set; }
        public string? GstCategoryName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveGstCategoryViewModel
    {
        public GstCategoryViewModel gstCategory { get; set; }
        public string? companyId { get; set; }
    }

    public class GstViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<GstViewModel> data { get; set; }
    }

    public class GstDtViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<GstDtViewModel> data { get; set; }
    }

    public class GstCategoryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<GstCategoryViewModel>? data { get; set; }
    }
}