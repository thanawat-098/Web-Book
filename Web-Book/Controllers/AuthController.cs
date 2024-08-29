using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Web_Book.Context;
using Web_Book.Models;

namespace Web_Book.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserDbContext _context;

        public AuthController(UserDbContext context) 
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                return BadRequest("ชื่อผู้ใช้อยู่แล้ว");
            }

            user.PasswordHash = ComputeSha256Hash(user.PasswordHash);
            user.Role = "พนักงาน";
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "ลงทะเบียนสำเร็จ", User = user });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var existngUser = _context.Users.SingleOrDefault(u => u.Username == loginModel.Username);
            if (existngUser == null || existngUser.PasswordHash != ComputeSha256Hash(loginModel.PasswordHash))
            {
                return Unauthorized(new {Message = "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง" } );
            }
            return Ok(new {Message = "เข้าสู่ระบบสำเร็จ", User = existngUser }); 
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
