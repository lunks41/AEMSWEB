namespace AEMSWEB.Models.Admin
{
    public class LoginViewModel
    {
        public string? userName { get; set; }
        public string? userPassword { get; set; }
        public BrowserInfo browserInfo { get; set; }
    }

    public class BrowserInfo
    {
        public string? name { get; set; }
        public string? os { get; set; }
        public string? ip { get; set; }
    }
}