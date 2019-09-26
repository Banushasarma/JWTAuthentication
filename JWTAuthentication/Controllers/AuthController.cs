using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration configuration;

        public AuthController(IConfiguration iconfig)
        {
            configuration = iconfig;
        }

        [HttpPost("token")]
        public ActionResult GetToken(string username, string password)
        {

            if (ValidateLogin(username, password))
            {
                //Security Key
                //string securityKey = "This is my security key";
                string securityKey = configuration.GetSection("Jwt").GetSection("SymmentricKey").Value;

                //Symmetric security key
                //var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

                //signing credientials
                var signingCredientials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                //create claims
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));

                //create token
                var token = new JwtSecurityToken(
                        issuer: configuration.GetSection("Jwt").GetSection("Issuer").Value,
                        audience: configuration.GetSection("Jwt").GetSection("Audience").Value,
                        expires: DateTime.Now.AddHours(1),
                        signingCredentials: signingCredientials,
                        claims: claims
                        );

                //return token
                return Ok(new JwtSecurityTokenHandler().WriteToken(token));


            }
            else
            {
                return BadRequest("Invalid credientials");
            }


        }

        private bool ValidateLogin(string username, string password)
        {
            if(username == "Banu" && password == "password")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}