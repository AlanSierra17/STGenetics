using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using STGeneticsTest.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace STGeneticsTest.Controllers
{

   [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        //private readonly IUserService _userService;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            //_userService = userService;
        }

        //[HttpGet, Authorize(Roles = "Admi")]
        //public ActionResult<string> GetMyName()
        //{
        //    return Ok(_userService.GetMyName());

        //    var userName = User?.Identity?.Name;
        //    var roleClaims = User?.FindAll(ClaimTypes.Role);
        //    var roles = roleClaims?.Select(c => c.Value).ToList();
        //    var roles2 = User?.Claims
        //        .Where(c => c.Type == ClaimTypes.Role)
        //        .Select(c => c.Value)
        //        .ToList();
        //    return Ok(new { userName, roles, roles2 });
        //}

        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request)
        {
            string passwordHash
                = BCrypt.Net.BCrypt.HashPassword(request.Password);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;

            return Ok(user);
        }

        [HttpPost("login")]
        public ActionResult<User> Login(UserDto request)
        {
            if (user.Username != request.Username)
            {
                return BadRequest("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Admin"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
