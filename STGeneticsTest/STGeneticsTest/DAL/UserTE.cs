using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using STGeneticsTest.Models;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace STGeneticsTest.DAL
{
    public class UserTE
    {
        private readonly IConfiguration _configuration;

        public UserTE(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreateUser(UserDto request)
        {
            string passwordHash
               = BCrypt.Net.BCrypt.HashPassword(request.Password);

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            int rowsAffected = await connection.ExecuteAsync("INSERT INTO[User](UserName, PasswordHash) VALUES (@UserName, @Password)", new
            {
                UserName = request.Username,
                Password = passwordHash
            });
            if (rowsAffected != 0)
                return "User successfully created.";
            else
                return "";

        }
        public async Task<string> Login(UserDto request)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            User user = await connection.QueryFirstOrDefaultAsync<User>("SELECT UserName, PasswordHash FROM [User] " +
                                                         "WHERE UserName = @UserName",
                                                         new
                                                         {
                                                             UserName = request.Username
                                                         });

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return "Wrong user or password.";
            }

            return CreateToken(user);
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
