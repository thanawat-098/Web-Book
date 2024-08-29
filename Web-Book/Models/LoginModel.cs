using System.ComponentModel.DataAnnotations;

namespace Web_Book.Models
{
    public class LoginModel
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }
    }
}
