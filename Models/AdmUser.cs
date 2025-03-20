using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Models
{
    public class AdmUser : IdentityUser<short>
    {
        public AdmUser()
        {
            // Ensure no critical property is null
            UserName = string.Empty;
            NormalizedUserName = string.Empty;
            Email = string.Empty;
            NormalizedEmail = string.Empty;
            SecurityStamp = Guid.NewGuid().ToString();
            ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserId")]
        public override short Id { get; set; }

        [Required]
        public override string UserName { get; set; }

        [Required]
        public override string NormalizedUserName { get; set; }

        [Required]
        public override string Email { get; set; }

        [Required]
        public override string NormalizedEmail { get; set; }

        public override string PasswordHash { get; set; }

        [Required]
        public override string SecurityStamp { get; set; }

        [Required]
        public override string ConcurrencyStamp { get; set; }

        public override DateTimeOffset? LockoutEnd { get; set; }
        public override string? PhoneNumber { get; set; }
        public override bool PhoneNumberConfirmed { get; set; }
        public override bool TwoFactorEnabled { get; set; }

        // Custom properties
        public string? FullName { get; set; }

        public string? UserCode { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }

        // Relationship mapping
        [ForeignKey(nameof(UserGroup))]
        public short UserGroupId { get; set; }

        public virtual AdmUserGroup UserGroup { get; set; }
    }
}