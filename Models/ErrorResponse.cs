namespace AMESWEB.Models
{
    public class ErrorResponse
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public string?[] data { get; set; }
    }
}