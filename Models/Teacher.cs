using System.ComponentModel.DataAnnotations;

namespace WebLab.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        [StringLength(120, MinimumLength = 1)]
        public string FullName { get; set; }

        [StringLength(200, MinimumLength = 1)]
        public string? Department { get; set; }

        [StringLength(200, MinimumLength = 1)]
        [EmailAddress]
        public string? Email { get; set; }

        [Range(0,100)]
        public int? ExperienceYears { get; set; }

        public bool? Active { get; set; }
    }
}
