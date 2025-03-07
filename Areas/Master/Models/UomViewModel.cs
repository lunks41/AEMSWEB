﻿namespace AEMSWEB.Models.Masters
{
    public class UomViewModel
    {
        public Int16 UomId { get; set; }
        public Int16 CompanyId { get; set; }
        public string UomCode { get; set; }
        public string UomName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveUomViewModel
    {
        public UomViewModel Uom { get; set; }
        public string CompanyId { get; set; }
    }
}