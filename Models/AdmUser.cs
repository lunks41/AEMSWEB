using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Models
{
    public class AdmUser : IdentityUser<short>
    {
        [Column("UserId")] // Maps the Id property to "UserId" column
        public override short Id { get; set; }

        // Custom properties
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }

        // Relationship

        [ForeignKey(nameof(UserGroup))]
        public short UserGroupId { get; set; }

        public virtual AdmUserGroup UserGroup { get; set; }
    }
}