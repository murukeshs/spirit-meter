using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        /// <summary>
        /// To createUser
        /// </summary>
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
        /// <summary>
        /// To updateUser
        /// </summary>
        [HttpPut, Route("updateUser")]
        public IActionResult updateUser([FromBody]createUser updateUser)
        {
            try
            {
                Regex regex = new Regex(@"^\-?\d+\.?\d*$");
                    if (updateUser.userId <= 0 | updateUser.userId == null)
                    {
                        return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter userId" });
                    }
                    if (!String.IsNullOrEmpty(updateUser.latitude ) )
                    {
                        System.Text.RegularExpressions.Match latitude = regex.Match(updateUser.latitude);
                        if(latitude.Success != true)
                        return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter valid latitude" });
                    }
                    if (!String.IsNullOrEmpty(updateUser.longitude ) )
                    {
                        System.Text.RegularExpressions.Match longitude = regex.Match(updateUser.latitude);
                        if (longitude.Success != true)
                         return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter valid longitude" });
                    }

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
        /// <summary>
        /// To deleteUser
        /// </summary>
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
        /// <summary>
        /// To listUser
        /// </summary>
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
                        listUser.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        listUser.savedDisplay = (dt.Rows[i]["savedDisplay"] == DBNull.Value ? 0 : (int)dt.Rows[i]["savedDisplay"]);
                        listUser.savedRoutes = (dt.Rows[i]["savedRoutes"] == DBNull.Value ? 0 : (int)dt.Rows[i]["savedRoutes"]);
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
        /// <summary>
        /// To  select UserById
        /// </summary>
        [HttpGet, Route("selectUserById/{userId}")]
        public IActionResult selectUserById(int userId)
        {
          
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
                    user.name = (dt.Rows[0]["name"] == DBNull.Value ? "" : dt.Rows[0]["name"].ToString());
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
                    user.createdDate = (dt.Rows[0]["createdDate"] == DBNull.Value ? "" : dt.Rows[0]["createdDate"].ToString());
                    user.password = DecryptPassword;

                    return StatusCode((int)HttpStatusCode.OK, user);
                }

                else
                {
                    string[] data = new string[0];
                    return StatusCode((int)HttpStatusCode.OK, user);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("selectUserById", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region listCharity
        /// <summary>
        /// To listCharity
        /// </summary>
        [HttpGet, Route("listCharity")]
        public IActionResult listCharity()
        {
            List<dynamic> userList = new List<dynamic>();
            try
            {

                DataTable dt = Data.User.ListCharity();

                dynamic user = new System.Dynamic.ExpandoObject();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        user.userId = (int)dt.Rows[i]["userId"];
                        user.name = (dt.Rows[i]["name"] == DBNull.Value ? "" : dt.Rows[i]["name"].ToString());
                        user.phoneNumber = (dt.Rows[i]["phoneNumber"] == DBNull.Value ? "" : dt.Rows[i]["phoneNumber"].ToString());
                        user.profileImage = (dt.Rows[i]["profileImage"] == DBNull.Value ? "" : dt.Rows[i]["profileImage"].ToString());
                        user.gender = (dt.Rows[i]["gender"] == DBNull.Value ? "" : dt.Rows[i]["gender"].ToString());
                        user.role = (dt.Rows[i]["role"] == DBNull.Value ? "" : dt.Rows[i]["role"].ToString());
                        user.latitude = (dt.Rows[i]["latitude"] == DBNull.Value ? "" : dt.Rows[i]["latitude"].ToString());
                        user.longitude = (dt.Rows[i]["longitude"] == DBNull.Value ? "" : dt.Rows[i]["longitude"].ToString());
                        user.country = (dt.Rows[i]["country"] == DBNull.Value ? "" : dt.Rows[i]["country"].ToString());
                        user.state = (dt.Rows[i]["state"] == DBNull.Value ? "" : dt.Rows[i]["state"].ToString());
                        user.cityName = (dt.Rows[i]["cityName"] == DBNull.Value ? "" : dt.Rows[i]["cityName"].ToString());
                        user.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());

                        userList.Add(user);
                    }

                    return StatusCode((int)HttpStatusCode.OK, userList);
                }

                else
                {
                    string[] data = new string[0];
                    return StatusCode((int)HttpStatusCode.OK, userList);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listCharity", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region forgetPassword
        /// <summary>
        /// To update forgetPassword
        /// </summary>
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
                        return StatusCode((int)HttpStatusCode.OK, new { message = "Updated Successfully" });
                    }
                    else if (row == "Invalid OTP")
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { errorMessage = "Invalid OTP" });
                    }
                    else if (row == "Invalid PhoneNumber")
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { errorMessage = "Invalid PhoneNumber" });
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

        #region phoneVerify
        /// <summary>
        /// To verify phone using OTP
        /// </summary>
        [HttpPut, Route("phoneVerify")]
        [AllowAnonymous]
        public IActionResult phoneVerify(phoneVerify phoneVerify)
        {
            try
            {
                if (phoneVerify.phone == "" || phoneVerify.phone == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter Phone" });
                }
                else if (phoneVerify.OTPValue == "" || phoneVerify.OTPValue == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter OTPValue" });
                }



                string row = Data.User.phoneVerify(phoneVerify);

                if (row == "Success")
                {
                    return StatusCode((int)HttpStatusCode.OK, new { message = "Phone Number Verified Successfully" });
                }
                else if (row == "Invalid PhoneNumber")
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { errorMessage = "Invalid PhoneNumber" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = row });
                }
            }

            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("phoneVerify", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion phoneVerify   

        #region generateOTP
        /// <summary>
        /// To generateOTP for forgotpassword and registration
        /// </summary>
        [HttpPut, Route("generateOTP")]
        [AllowAnonymous]
        public IActionResult generateOTP([FromBody]GenerateOTP otp)
        {
            try
            {
                string OTPValue = Common.GenerateOTP();

                SMSResponse results = new SMSResponse();

                var message = "";

                //otp.emailorPhone = "+14087224019";

                string SaveOtpValue = Data.User.GenerateOTP(OTPValue, otp);

                if (SaveOtpValue == "Success")
                {
                    results = SmsNotification.SendMessage(otp.phone, "Hi User, your OTP is " + OTPValue + " and it's expiry time is 15 minutes.");

                    string status = results.messages[0].status.ToString();

                    if (status == "0")
                    {
                        message = "Message sent successfully.";
                    }
                    else
                    {
                        string err = results.messages[0].error_text.ToString();
                        message = err;
                    }


                    return StatusCode((int)HttpStatusCode.OK, new { message });      
                }

                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Phone number not available" });
                }

            }

            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("generateOTP", e.Message.ToString());

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion

        #region spiritMeter
        /// <summary>
        /// To show spiritMeter Status
        /// </summary>
        [HttpGet, Route("spiritMeter/{userId}")]
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

        #region selectUser
        /// <summary>
        /// To selectUser by userId
        /// </summary>
        [HttpGet, Route("selectUser/{userId}")]
        public IActionResult selectUser(int userId)
        {
            List<dynamic> listDisplayDetails = new List<dynamic>();
            dynamic listUser = new System.Dynamic.ExpandoObject();
            try
            {
                DataSet ds = Data.User.selectBasicUserById(userId);

                List<displayList> listDisplay = new List<displayList>();
                displayLists Displays = new displayLists();
                if(ds.Tables[1].Rows.Count > 0)
                { 
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    displayList Display = new displayList();

                    Display.displayId = (int)ds.Tables[1].Rows[i]["displayId"];
                    Display.name = (ds.Tables[1].Rows[i]["name"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["name"].ToString());
                    Display.categoryId = (ds.Tables[1].Rows[i]["categoryId"] == DBNull.Value ? 0 : (int)ds.Tables[1].Rows[i]["categoryId"]);
                    Display.categoryName = (ds.Tables[1].Rows[i]["categoryName"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["categoryName"].ToString());
                    Display.notes = (ds.Tables[1].Rows[i]["notes"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["notes"].ToString());
                    Display.latitude = (ds.Tables[1].Rows[i]["latitude"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["latitude"].ToString());
                    Display.longitude = (ds.Tables[1].Rows[i]["longitude"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["longitude"].ToString());
                    Display.country = (ds.Tables[1].Rows[i]["country"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["country"].ToString());
                    Display.state = (ds.Tables[1].Rows[i]["state"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["state"].ToString());
                    Display.cityName = (ds.Tables[1].Rows[i]["cityName"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["cityName"].ToString());
                    Display.address = (ds.Tables[1].Rows[i]["address"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["address"].ToString());
                    Display.type = (ds.Tables[1].Rows[i]["type"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["type"].ToString());
                    Display.isPrivate = (ds.Tables[1].Rows[i]["isPrivate"] == DBNull.Value ? false : (bool)ds.Tables[1].Rows[i]["isPrivate"]);
                    Display.createdDate = (ds.Tables[1].Rows[i]["createdDate"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["createdDate"].ToString());
                    Display.createdBy = (ds.Tables[1].Rows[i]["createdBy"] == DBNull.Value ? 0 : (int)ds.Tables[1].Rows[i]["createdBy"]);
                    Display.createdByName = (ds.Tables[1].Rows[i]["createdName"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["createdName"].ToString());
                    Display.filePath = (ds.Tables[1].Rows[i]["filePath"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["filePath"].ToString());
                    Display.routes = (ds.Tables[1].Rows[i]["routes"] == DBNull.Value ? "" : ds.Tables[1].Rows[i]["routes"].ToString());

                    listDisplay.Add(Display);
                }                    
                    listUser.userId = (int)ds.Tables[0].Rows[0]["userId"];
                    listUser.name = (ds.Tables[0].Rows[0]["name"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["name"].ToString());
                    listUser.phoneNumber = (ds.Tables[0].Rows[0]["phoneNumber"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["phoneNumber"].ToString());
                    listUser.profileImage = (ds.Tables[0].Rows[0]["profileImage"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["profileImage"].ToString());
                    listUser.gender = (ds.Tables[0].Rows[0]["gender"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["gender"].ToString());
                    listUser.role = (ds.Tables[0].Rows[0]["role"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["role"].ToString());
                    listUser.latitude = (ds.Tables[0].Rows[0]["latitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["latitude"].ToString());
                    listUser.longitude = (ds.Tables[0].Rows[0]["longitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["longitude"].ToString());
                    listUser.country = (ds.Tables[0].Rows[0]["country"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["country"].ToString());
                    listUser.state = (ds.Tables[0].Rows[0]["state"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["state"].ToString());
                    listUser.cityName = (ds.Tables[0].Rows[0]["cityName"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["cityName"].ToString());
                    listUser.address = (ds.Tables[0].Rows[0]["address"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["address"].ToString());
                    listUser.createdDate = (ds.Tables[0].Rows[0]["createdDate"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["createdDate"].ToString());

                    List<displayList> list = listDisplay.Where(a => a.createdBy == (int)ds.Tables[0].Rows[0]["userId"]).ToList();
                    if (list.Count > 0)
                    {
                        listUser.displayList = list;

                    }
                    else
                    {
                        listUser.displayList = "";

                    }

                    List<dynamic> listRoute = new List<dynamic>();
                   
                    for (int j = 0; j < ds.Tables[2].Rows.Count; j++)
                    {
                        dynamic route = new System.Dynamic.ExpandoObject();
                        route.routeId = (int)ds.Tables[2].Rows[j]["routeId"];
                        route.routeName = (ds.Tables[2].Rows[j]["routeName"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["routeName"].ToString());
                        route.comments = (ds.Tables[2].Rows[j]["comments"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["comments"].ToString());
                        route.designatedCharityId = (ds.Tables[2].Rows[j]["designatedCharityId"] == DBNull.Value ? 0 : (int)ds.Tables[2].Rows[j]["designatedCharityId"]);
                        route.designatedCharityName = (ds.Tables[2].Rows[j]["designatedCharityName"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["designatedCharityName"].ToString());
                        route.isPrivate = (ds.Tables[2].Rows[j]["isPrivate"] == DBNull.Value ? false : (bool)ds.Tables[2].Rows[j]["isPrivate"]);
                        route.createdDate = (ds.Tables[0].Rows[j]["createdDate"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["createdDate"].ToString());
                        route.startingPoint = (ds.Tables[2].Rows[j]["startingPoint"] == DBNull.Value ? 0 : (int)ds.Tables[2].Rows[j]["startingPoint"]);
                        route.latitude = (ds.Tables[2].Rows[j]["latitude"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["latitude"].ToString());
                        route.longitude = (ds.Tables[2].Rows[j]["longitude"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["longitude"].ToString());
                        route.country = (ds.Tables[2].Rows[j]["country"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["country"].ToString());
                        route.state = (ds.Tables[2].Rows[j]["state"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["state"].ToString());
                        route.cityName = (ds.Tables[2].Rows[j]["cityName"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["cityName"].ToString());
                        route.address = (ds.Tables[2].Rows[j]["address"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["address"].ToString());
                        route.routePoints = (ds.Tables[2].Rows[j]["routePoints"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["routePoints"].ToString());
                        route.totalMiles = (ds.Tables[2].Rows[j]["totalMiles"] == DBNull.Value ? "" : String.Concat(ds.Tables[2].Rows[j]["totalMiles"], "mi").ToString());
                        //route.path = (ds.Tables[2].Rows[j]["path"] == DBNull.Value ? "" : ds.Tables[2].Rows[j]["path"].ToString());
                        listRoute.Add(route);
                    }
                    listUser.routeList = listRoute;
                    
                return StatusCode((int)HttpStatusCode.OK, listUser);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, listUser);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("selectUser", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region getProfile
        /// <summary>
        /// To getProfile by userId
        /// </summary>
        [HttpGet, Route("getProfile/{userId}")]
        public IActionResult getProfile(int userId)
        {
            try
            {
                DataSet ds = Data.User.getProfile(userId);
               
                    dynamic user = new System.Dynamic.ExpandoObject();
                if(ds.Tables[0].Rows.Count > 0)
                {
                    user.userId = (int)ds.Tables[0].Rows[0]["userId"];
                    user.name = (ds.Tables[0].Rows[0]["name"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["name"].ToString());
                    user.phoneNumber = (ds.Tables[0].Rows[0]["phoneNumber"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["phoneNumber"].ToString());
                    user.profileImage = (ds.Tables[0].Rows[0]["profileImage"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["profileImage"].ToString());
                    user.gender = (ds.Tables[0].Rows[0]["gender"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["gender"].ToString());
                    user.role = (ds.Tables[0].Rows[0]["role"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["role"].ToString());
                    user.latitude = (ds.Tables[0].Rows[0]["latitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["latitude"].ToString());
                    user.longitude = (ds.Tables[0].Rows[0]["longitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["longitude"].ToString());
                    user.country = (ds.Tables[0].Rows[0]["country"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["country"].ToString());
                    user.state = (ds.Tables[0].Rows[0]["state"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["state"].ToString());
                    user.cityName = (ds.Tables[0].Rows[0]["cityName"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["cityName"].ToString());
                    user.address = (ds.Tables[0].Rows[0]["address"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["address"].ToString());
                    user.createdDate = (ds.Tables[0].Rows[0]["createdDate"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["createdDate"].ToString());
                    user.savedRoutes = (ds.Tables[2].Rows[0]["savedRoutes"] == DBNull.Value ? 0 : (int)ds.Tables[2].Rows[0]["savedRoutes"]);
                    user.savedDisplay = (ds.Tables[1].Rows[0]["savedDisplay"] == DBNull.Value ? 0 : (int)ds.Tables[1].Rows[0]["savedDisplay"]);
                    user.spiritPoints = (ds.Tables[3].Rows[0]["spiritPoints"] == DBNull.Value ? 0 : (int)ds.Tables[3].Rows[0]["spiritPoints"]);

                 return StatusCode((int)HttpStatusCode.OK, user);
                }
                else
                {
                 return StatusCode((int)HttpStatusCode.OK, user);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getProfile", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion


    }
}