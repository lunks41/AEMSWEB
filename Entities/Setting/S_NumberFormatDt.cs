using Microsoft.EntityFrameworkCore;

namespace AEMSWEB.Entities.Setting
{
    [PrimaryKey(nameof(NumberId), nameof(NumYear))]
    public class S_NumberFormatDt
    {
        public Int32 NumberId { get; set; }
        public Int32 NumYear { get; set; }
        public Int32 Month1 { get; set; }
        public Int32 Month2 { get; set; }
        public Int32 Month3 { get; set; }
        public Int32 Month4 { get; set; }
        public Int32 Month5 { get; set; }
        public Int32 Month6 { get; set; }
        public Int32 Month7 { get; set; }
        public Int32 Month8 { get; set; }
        public Int32 Month9 { get; set; }
        public Int32 Month10 { get; set; }
        public Int32 Month11 { get; set; }
        public Int32 Month12 { get; set; }
        public Int32 LastNumber { get; set; }
    }
}