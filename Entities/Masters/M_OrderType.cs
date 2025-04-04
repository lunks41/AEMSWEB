﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Masters
{
    public class M_OrderType
    {
        public Int16 CompanyId { get; set; }

        [Key]
        public Int16 OrderTypeId { get; set; }

        public string? OrderTypeCode { get; set; }
        public string? OrderTypeName { get; set; }

        [ForeignKey("OrderTypeCategoryId")]
        public Int16 OrderTypeCategoryId { get; set; }

        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}