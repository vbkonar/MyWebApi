using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private MyWebApiContext _context;

        public AuthenticationController(MyWebApiContext context)
        {
            _context = context;
        }

        // POST api/values
        [HttpPost]
        public ActionResult<string> Post([FromBody] User userObj)
        {

            var user = _context.Users.FirstOrDefault(x => x.Name.Equals(userObj.Name) && x.Password.Equals(userObj.Password));
            if (user != null) {
                //return "Login Successful";
                var tokenHandler = new JwtSecurityTokenHandler();
                //TODO: Add Encryption/Decryption Mechanism for key
                // Move key to appConfig
                var key = Encoding.ASCII.GetBytes("THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET, IT CAN BE ANY STRING");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);

            }
            else {
                return "Login not successful";
            }
        }
    }
}