namespace AEMSWEB.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string? CompanyId { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}