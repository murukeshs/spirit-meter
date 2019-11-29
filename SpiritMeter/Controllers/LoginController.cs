using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SpiritMeter.Models;

namespace SpiritMeter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }


        private string GenerateJSONWebToken()  //Login userInfo
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            IConfigurationRoot configuration = builder.Build();
            var JwtKey = configuration.GetSection("Jwt").GetSection("Key").Value;

            var JwtIssuer = configuration.GetSection("Jwt").GetSection("Issuer").Value;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(JwtIssuer,
            JwtIssuer,
            null,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #region GetUserLogin     
        // GET api/values
        [HttpPost, Route("Login")]
        public IActionResult Login([FromBody]Login login)
        {
            IActionResult response = Unauthorized();
            List<dynamic> userdetails = new List<dynamic>();
            try
            {
                DataSet ds = Data.User.login(login);
                DataTable dt = ds.Tables[0];
                dynamic user = new System.Dynamic.ExpandoObject();
                string token = "";

                if (dt.Rows.Count > 0)
                {
                    token = GenerateJSONWebToken();
                    user.userId = (int)dt.Rows[0]["userId"];
                    user.firstName = (dt.Rows[0]["firstName"] == DBNull.Value ? "" : dt.Rows[0]["firstName"].ToString());
                    user.lastName = (dt.Rows[0]["lastName"] == DBNull.Value ? "" : dt.Rows[0]["lastName"].ToString());
                    user.phoneNumber = (dt.Rows[0]["phoneNumber"] == DBNull.Value ? "" : dt.Rows[0]["phoneNumber"].ToString());
                    user.profileImage = (dt.Rows[0]["profileImage"] == DBNull.Value ? "" : dt.Rows[0]["profileImage"].ToString());
                    user.gender = (dt.Rows[0]["gender"] == DBNull.Value ? "" : dt.Rows[0]["gender"].ToString());
                    user.role = (dt.Rows[0]["role"] == DBNull.Value ? "" : dt.Rows[0]["role"].ToString());
                    user.latitude = (dt.Rows[0]["latitude"] == DBNull.Value ? "" : dt.Rows[0]["latitude"].ToString());
                    user.longitude = (dt.Rows[0]["longitude"] == DBNull.Value ? "" : dt.Rows[0]["longitude"].ToString());
                    user.country = (dt.Rows[0]["country"] == DBNull.Value ? "" : dt.Rows[0]["country"].ToString());
                    user.state = (dt.Rows[0]["state"] == DBNull.Value ? "" : dt.Rows[0]["state"].ToString());
                    user.cityName = (dt.Rows[0]["cityName"] == DBNull.Value ? "" : dt.Rows[0]["cityName"].ToString());
                    user.address = (dt.Rows[0]["address"] == DBNull.Value ? "" : dt.Rows[0]["address"].ToString());
                   

                    return StatusCode((int)HttpStatusCode.OK, new { user, token });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Email & Password combination not found" });
                }

            }

            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

    }
}