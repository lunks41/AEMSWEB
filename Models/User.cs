namespace AEMSWEB.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; } // Store hashed passwords, not plain text
        public int CompanyId { get; set; }
    }

    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int CompanyId { get; set; }
    }
}