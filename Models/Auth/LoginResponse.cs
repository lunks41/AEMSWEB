namespace AEMSWEB.Models.Auth
{
    public class LoginResponse
    {
        public string? token { get; set; }
        public string? refreshToken { get; set; }
    }
}