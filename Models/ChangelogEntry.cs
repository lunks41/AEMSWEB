using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Models
{
    public class ChangelogEntry
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Version { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ChangeType { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        public DateTime CreatedDate { get; set; }

        [Url]
        [StringLength(500)]
        public string ReferenceURL { get; set; }

        public bool IsPublic { get; set; }

        [StringLength(20)]
        public string SystemVersion { get; set; }
    }
}