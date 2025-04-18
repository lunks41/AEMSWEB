﻿using AMESWEB.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Areas.Account.Models.AR
{
    public class ARDocSetOffViewModel
    {
        private DateTime _trnDate;
        private DateTime _accountDate;

        public short CompanyId { get; set; }
        public string? SetoffId { get; set; }
        public string? SetoffNo { get; set; }
        public string? ReferenceNo { get; set; }

        public string? TrnDate
        {
            get { return DateHelperStatic.FormatDate(_trnDate); }
            set { _trnDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string? AccountDate
        {
            get { return DateHelperStatic.FormatDate(_accountDate); }
            set { _accountDate = DateHelperStatic.ParseDBDate(value); }
        }

        public int CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public short CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BalanceAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AllocAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal UnAllocAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExhGainLoss { get; set; }

        public string? Remarks { get; set; }
        public string? ModuleFrom { get; set; }
        public string? CreateBy { get; set; }
        public short CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public short? EditById { get; set; }
        public string? EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public short? CancelById { get; set; }
        public string? CancelBy { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancelRemarks { get; set; }
        public byte EditVersion { get; set; }
        public List<ARDocSetOffDtViewModel> data_details { get; set; }
    }
}