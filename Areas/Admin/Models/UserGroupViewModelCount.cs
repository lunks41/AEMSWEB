﻿namespace AMESWEB.Models.Admin
{
    public class UserGroupViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<UserGroupViewModel> data { get; set; }
    }
}