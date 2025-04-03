namespace AMESWEB.Models.Utilites
{
    public class PagingViewModel
    {
        public Int16 pageSize { get; set; }
        public Int16 pageNumber { get; set; }
        public string? searchString { get; set; }
    }
}