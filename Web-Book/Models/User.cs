using System.ComponentModel.DataAnnotations;

namespace Web_Book.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(50)]
        public string Role { get; set; }
    }
}
