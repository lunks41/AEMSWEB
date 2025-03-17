namespace AEMSWEB.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations.Schema;

    public class AdmUserGroup : IdentityRole<short>
    {
        [Column("UserGroupId")] // Maps the Id property to "UserId" column
        public override short Id { get; set; }

        public string UserGroupCode { get; set; }
        public string UserGroupName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}