using System.Net;

namespace AEMSWEB.Models
{
    public class Response
    {
        public bool IsSuccess { get; set; } = false;
        public bool IslogedIn { get; set; } = false;
        public string? JwtToken { get; set; }
        public string? Message { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}