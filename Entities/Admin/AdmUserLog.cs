using Microsoft.EntityFrameworkCore;

namespace AEMSWEB.Entities.Admin
{
    [PrimaryKey(nameof(UserId), nameof(LoginDate))]
    public class AdmUserLog
    {
        public Int16 UserId { get; set; }
        public bool IsLogin { get; set; }
        public DateTime LoginDate { get; set; }
        public string? Remarks { get; set; }
    }
}