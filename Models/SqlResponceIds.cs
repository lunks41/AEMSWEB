namespace AMESWEB.Models
{
    public class SqlResponseIds
    {
        public Int64 NextId { get; set; }//NextId
        public Int64 CountId { get; set; }
        public Int32 IsExist { get; set; }
        public Int64 DocumentId { get; set; }
        public string? DocumentNo { get; set; }
    }

    // Supporting class
    public class ExistenceResult
    {
        public int CodeExists { get; set; }
        public int NameExists { get; set; }
    }
}