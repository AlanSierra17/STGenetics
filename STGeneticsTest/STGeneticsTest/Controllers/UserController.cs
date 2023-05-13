using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using STGeneticsTest.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace STGeneticsTest.Controllers
{

   [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("register")]
        public async Task<ActionResult<DefaultResponse<string>>> CreateAnimal(UserDto request)
        {
            DefaultResponse<string> response = new DefaultResponse<string>();
            try
            {
                DAL.UserTE userTE = new DAL.UserTE(_configuration);
                string res = await userTE.CreateUser(request);

                if(res == string.Empty) {
                    return BadRequest("Failed to create user");
                }
                else
                {
                    response.Data = "User successfully created.";
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Msg = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<DefaultResponse<string>>> Login(UserDto request)
        {
            DefaultResponse<string> response = new DefaultResponse<string>();

            try
            {
                DAL.UserTE userTE = new DAL.UserTE(_configuration);
                string res = await userTE.Login(request);

                response.Data = res;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Msg = ex.Message;
            }
            return Ok(response);
        }
    }
}
