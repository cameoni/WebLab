using System.ComponentModel.DataAnnotations;

namespace WebLab.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool EditAccess { get; set; }
    }
}
