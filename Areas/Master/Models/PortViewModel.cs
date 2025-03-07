﻿namespace AEMSWEB.Areas.Master.Models
{
    public class PortViewModel
    {
        public Int16 PortId { get; set; }
        public Int16 CompanyId { get; set; }
        public Int16 PortRegionId { get; set; }
        public string PortRegionCode { get; set; }
        public string PortRegionName { get; set; }
        public string PortCode { get; set; }
        public string PortName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}