namespace AEMSWEB.Models.Admin
{
    public class UserLogViewModel
    {
        public Int16 UserId { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public bool IsLogin { get; set; }
        public DateTime LoginDate { get; set; }
        public string? Remarks { get; set; }
    }
}