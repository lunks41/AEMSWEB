namespace AMESWEB.Models
{
    public class BaseViewModel<T>
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
    }
}