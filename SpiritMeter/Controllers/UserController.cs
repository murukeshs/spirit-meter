using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SpiritMeter.Data;
using SpiritMeter.Models;
using static Nexmo.Api.SMS;

namespace SpiritMeter.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        #region createUser
        [HttpPost, Route("createUser")]
        [AllowAnonymous]
        public IActionResult createUser(createUser createUser)
        {
            try
            {
                if (createUser.firstName == "" || createUser.firstName == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter First Name" });
                }
                else if (createUser.password == "" || createUser.password == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter Password" });
                }
                else if (createUser.lastName == "" ||  createUser.lastName == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter lastName" });
                }
                else if (createUser.phoneNumber == "" || createUser.phoneNumber == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter phonenumber" });
                }

                Regex regex = new Regex(@"^\-?\d+\.?\d*$");
                System.Text.RegularExpressions.Match latitude = regex.Match(createUser.latitude);
                System.Text.RegularExpressions.Match longitude = regex.Match(createUser.latitude);
                if (latitude.Success & longitude.Success)
                {
                    DataTable dt = Data.User.createUser(createUser);


                    string Response = dt.Rows[0][0].ToString();

                    if (Response == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Saved Successfully");
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = Response });
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Invalid latitude/longitude" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("createUser", e.Message);
                if (e.Message.Contains("UQ__tblUser__4849DA01F0AAFB4B"))   // Check Duplicate Key for PhoneNo
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "PhoneNo is already registered" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
                }
            }
        }
        #endregion createUser

        #region updateUser
        [HttpPut, Route("updateUser")]
        public IActionResult updateUser([FromBody]createUser updateUser)
        {
            try
            {

                Regex regex = new Regex(@"^\-?\d+\.?\d*$");
                System.Text.RegularExpressions.Match latitude = regex.Match(updateUser.latitude);
                System.Text.RegularExpressions.Match longitude = regex.Match(updateUser.latitude);
                if (latitude.Success & longitude.Success)
                {
                    DataTable dt = Data.User.updateUser(updateUser);
                    string Response = dt.Rows[0][0].ToString();
                    if (Response == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                    }
                    else
                    {
                        if (Response.Contains("UQ__tblUser__4849DA01F0AAFB4B") == true)
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Phone No is already taken" });
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = Response });
                        }
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Invalid latitude/longitude" });
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateUser", e.Message);
                if (e.Message.Contains("UQ__tblUser__4849DA01F0AAFB4B") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Phone No is already taken" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
                }
            }
        }
        #endregion

        #region deleteTeam
        [HttpDelete, Route("deleteUser")]
        public IActionResult deleteUser(int UserID)
        {
            try
            {
                if (UserID <= 0 )
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter UserID" });
                }
                else
                {
                    DataTable dt = Data.User.deleteUser(UserID);
                    string Response = dt.Rows[0][0].ToString();
                    if (Response == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Deleted Successfully");
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Deletion failed" });
                    }
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("deleteUser", e.Message.ToString());

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion

        #region listUser
        [HttpGet, Route("listUser")]
        public IActionResult listUser(string SearchTerm)
        {
            List<dynamic> listUserDetails = new List<dynamic>();

            try
            {
                DataTable dt = Data.User.listUser(SearchTerm);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic listUser = new System.Dynamic.ExpandoObject();
                        listUser.userId = (int)dt.Rows[i]["userId"];
                        listUser.name = (dt.Rows[i]["name"] == DBNull.Value ? "" : dt.Rows[i]["name"].ToString());
                        listUser.phoneNumber = (dt.Rows[i]["phoneNumber"] == DBNull.Value ? "" : dt.Rows[i]["phoneNumber"].ToString());
                        listUser.profileImage = (dt.Rows[i]["profileImage"] == DBNull.Value ? "" : dt.Rows[i]["profileImage"].ToString());
                        listUser.gender = (dt.Rows[i]["gender"] == DBNull.Value ? "" : dt.Rows[i]["gender"].ToString());
                        listUser.role = (dt.Rows[i]["role"] == DBNull.Value ? "" : dt.Rows[i]["role"].ToString());
                        listUser.latitude = (dt.Rows[i]["latitude"] == DBNull.Value ? "" : dt.Rows[i]["latitude"].ToString());
                        listUser.longitude = (dt.Rows[i]["longitude"] == DBNull.Value ? "" : dt.Rows[i]["longitude"].ToString());
                        listUser.country = (dt.Rows[i]["country"] == DBNull.Value ? "" : dt.Rows[i]["country"].ToString());
                        listUser.state = (dt.Rows[i]["state"] == DBNull.Value ? "" : dt.Rows[i]["state"].ToString());
                        listUser.cityName = (dt.Rows[i]["cityName"] == DBNull.Value ? "" : dt.Rows[i]["cityName"].ToString());
                        listUser.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        listUser.createddate = (dt.Rows[i]["createddate"] == DBNull.Value ? "" : dt.Rows[i]["createddate"].ToString());
                       
                        listUserDetails.Add(listUser);
                    }
                    return StatusCode((int)HttpStatusCode.OK, listUserDetails);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, listUserDetails);
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listUser", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region selectUserById
        [HttpGet, Route("selectUserById/{userId}")]
        public IActionResult selectUserById(int userId)
        {
            List<dynamic> userList = new List<dynamic>();
            try
            {
                DataTable dt = Data.User.selectUserById(userId);
               
                dynamic user = new System.Dynamic.ExpandoObject();
                if (dt.Rows.Count > 0)
                {
                    var DecryptPassword = "";
                    if (dt.Rows[0]["password"].ToString() != "")
                    {
                        DecryptPassword = Common.DecryptData(dt.Rows[0]["password"] == DBNull.Value ? "" : dt.Rows[0]["password"].ToString());
                    }
                    else
                    {
                        DecryptPassword = "";
                    }
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
                    user.password = DecryptPassword;

                    userList.Add(user);

                    return StatusCode((int)HttpStatusCode.OK, user);
                }

                else
                {
                    string[] data = new string[0];
                    return StatusCode((int)HttpStatusCode.OK, data);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("selectUserById", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region ListCharity
        [HttpGet, Route("ListCharity")]
        public IActionResult ListCharity()
        {
            List<dynamic> userList = new List<dynamic>();
            try
            {
                DataTable dt = Data.User.ListCharity();

                dynamic user = new System.Dynamic.ExpandoObject();
                if (dt.Rows.Count > 0)
                {
                    user.userId = (int)dt.Rows[0]["userId"];
                    user.name = (dt.Rows[0]["name"] == DBNull.Value ? "" : dt.Rows[0]["name"].ToString());
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
                   
                    userList.Add(user);

                    return StatusCode((int)HttpStatusCode.OK, user);
                }

                else
                {
                    string[] data = new string[0];
                    return StatusCode((int)HttpStatusCode.OK, data);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("ListCharity", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region forgetPassword
        [HttpPut, Route("forgetPassword")]
        [AllowAnonymous]
        public IActionResult forgetPassword(forgotPassword forgotPassword)
        {
            try
            {
                if (forgotPassword.password == "" || forgotPassword.password == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter Password" });
                }
                else if (forgotPassword.phone == "" || forgotPassword.phone == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter Phone" });
                }
                else if (forgotPassword.OTPValue == "" || forgotPassword.OTPValue == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter OTPValue" });
                }
               
               
               
                    string row = Data.User.forgotPassword(forgotPassword);

                    if (row == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = row });
                    }
            }
                
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("forgetPassword", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion forgetPassword   
     
        #region GenerateOTP
        [HttpPut, Route("GenerateOTP")]
        [AllowAnonymous]
        public IActionResult generateOTP([FromBody]GenerateOTP otp)
        {
            try
            {
                string OTPValue = Common.GenerateOTP();

                SMSResponse results = new SMSResponse();

                var SmsStatus = "";

                //otp.emailorPhone = "+14087224019";

                string SaveOtpValue = Data.User.GenerateOTP(OTPValue, otp);

                if (SaveOtpValue == "Success")
                {
                    results = SmsNotification.SendMessage(otp.phone, "Hi User, your OTP is " + OTPValue + " and it's expiry time is 15 minutes.");

                    string status = results.messages[0].status.ToString();

                    if (status == "0")
                    {
                        SmsStatus = "Message sent successfully.";
                    }
                    else
                    {
                        string err = results.messages[0].error_text.ToString();
                        SmsStatus = err;
                    }


                    return StatusCode((int)HttpStatusCode.OK, new { SmsStatus });       //results.messages, 
                }

                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Phone number not available" });
                }

            }

            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("SmsOTP", e.Message.ToString());

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion

        #region spiritMeter
        [HttpGet, Route("spiritMeter/userId")]
        public IActionResult spiritMeter(int userId)
        {
           
            try
            {
                DataTable dt = Data.User.spiritMeter();

                dynamic user = new System.Dynamic.ExpandoObject();
                if (dt.Rows.Count > 0)
                {
                    user.spiritMeter = (int)dt.Rows[0]["spiritMeter"];

                    return StatusCode((int)HttpStatusCode.OK, user);
                }

                else
                {
                    string[] data = new string[0];
                    return StatusCode((int)HttpStatusCode.OK, data);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("spiritMeter", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

    }
}