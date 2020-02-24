using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
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


        private List<KeyValuePair<string, dynamic>> GenerateJSONWebToken(string refreshToken)  //Login userInfo
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            IConfigurationRoot configuration = builder.Build();
            var JwtKey = configuration.GetSection("Jwt").GetSection("Key").Value;

            var JwtIssuer = refreshToken; //configuration.GetSection("Jwt").GetSection("Issuer").Value;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(JwtKey,
            JwtIssuer,
            null,   
            expires: DateTime.Now.AddMinutes(3),
            signingCredentials: credentials);
            var a = token.Claims.ToList()[0];
            var myList = new List<KeyValuePair<string, dynamic>>();
            myList.Add(new KeyValuePair<string, dynamic>("token", new JwtSecurityTokenHandler().WriteToken(token)));
            myList.Add(new KeyValuePair<string, dynamic>("exp", a.Value));
            return myList;
          
        }
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        #region GetUserLogin 
        /// <summary>
        /// To Login
        /// </summary>
        // GET api/values
        [HttpPost, Route("login")]
        public IActionResult login([FromBody]Login login)
        {
            IActionResult response = Unauthorized();
            List<dynamic> userdetails = new List<dynamic>();
            try
            {
                DataSet ds = Data.User.login(login);
                DataTable dt = ds.Tables[0];
                dynamic user = new System.Dynamic.ExpandoObject();
               
                if (dt.Rows.Count > 0)
                {  
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

                    //DateTime time = DateTime.Now.AddMinutes(3);
                    accessToken accessToken = new accessToken();
                    accessToken.refreshToken = GenerateRefreshToken();
                    var a =  GenerateJSONWebToken(accessToken.refreshToken);
                    accessToken.token = a[0].Value;
                    accessToken.expireIn = a[1].Value;
                    return StatusCode((int)HttpStatusCode.OK, new { user, accessToken });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Invalid Username or Password" });
                }
            }

            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("login", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region refreshToken
        /// <summary>
        /// To create refreshToken
        /// </summary>
        [HttpPost, Route("refreshToken")]
        public IActionResult refreshToken([FromBody]RefreshRequest login)
        {
            try
            {             
                if (HttpContext.User.Claims.ToList()[2].Value == login.refreshToken & HttpContext.Request.Headers["Authorization"][0].Split(' ')[1] == login.token)
                {
                    DateTime time = DateTime.Now.AddMinutes(3);
                    accessToken accessToken = new accessToken();
                    accessToken.refreshToken = GenerateRefreshToken();
                    var a = GenerateJSONWebToken(accessToken.refreshToken);
                    accessToken.token = a[0].Value;
                    accessToken.expireIn = a[1].Value;

                    return StatusCode((int)HttpStatusCode.OK, new { accessToken });
                }

                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter a valid Email" });
                }

            }

            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("refreshToken", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion
    }
}