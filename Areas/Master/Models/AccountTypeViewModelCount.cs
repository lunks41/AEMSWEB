﻿namespace AEMSWEB.Models.Masters
{
    public class AccountTypeViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<AccountTypeViewModel> data { get; set; }
    }
}