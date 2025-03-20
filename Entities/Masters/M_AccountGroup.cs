﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    public class M_AccountGroup
    {
        [Key]
        public Int16 AccGroupId { get; set; }

        public Int16 CompanyId { get; set; }
        public string? AccGroupCode { get; set; }
        public string? AccGroupName { get; set; }
        public Int16 SeqNo { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}