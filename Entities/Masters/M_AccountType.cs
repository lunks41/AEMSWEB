﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Masters
{
    public class M_AccountType
    {
        [Key]
        public Int16 AccTypeId { get; set; }

        public Int16 CompanyId { get; set; }
        public string? AccTypeCode { get; set; }
        public string? AccTypeName { get; set; }
        public Int32 CodeStart { get; set; }
        public Int32 CodeEnd { get; set; }
        public Int16 SeqNo { get; set; }
        public string? AccGroupName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}