using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpiritMeter.Data;
using SpiritMeter.Models;
using static SpiritMeter.Models.CharityModel;

namespace SpiritMeter.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CharityController : ControllerBase
    {

        #region createCharity
        /// <summary>
        /// To createCharity
        /// </summary>
        [HttpPost, Route("createCharity")]
        [AllowAnonymous]
        public IActionResult createCharity(CreateCharity createCharity)
        {
            try
            {
                if (createCharity.firstName == "" || createCharity.firstName == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter First Name" });
                }
                else if (createCharity.lastName == "" || createCharity.lastName == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter lastName" });
                }
                else if (createCharity.email == "" || createCharity.email == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter Email" });
                }
                else if (!String.IsNullOrEmpty(createCharity.email))
                {
                    Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    System.Text.RegularExpressions.Match Email = regexEmail.Match(createCharity.email);
                    if(Email.Success == false)
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter valid Email" });
                }
                else if (createCharity.phoneNumber == "" || createCharity.phoneNumber == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter phonenumber" });
                }
                
                    DataTable dt = Data.Charity.createCharity(createCharity);


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
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("createCharity", e.Message);
                if (e.Message.Contains("UQ__tblChari__4849DA01906D6338"))   // Check Duplicate Key for PhoneNumber
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "PhoneNo is already registered" });
                }
                if (e.Message.Contains("UQ__tblChari__AB6E6164E0E77029"))   // Check Duplicate Key for PhoneNumber
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Email is already registered" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
                }
            }
        }
        #endregion 

        #region updateCharity
        /// <summary>
        /// To updateCharity
        /// </summary>
        [HttpPut, Route("updateCharity")]
        public IActionResult updateCharity([FromBody]UpdateCharity updateCharity)
        {
            try
            {
                if (!String.IsNullOrEmpty(updateCharity.email))
                {
                    Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    System.Text.RegularExpressions.Match Email = regexEmail.Match(updateCharity.email);
                    if (Email.Success == false)
                        return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter valid Email" });
                }

                DataTable dt = Data.Charity.updateCharity(updateCharity);
                string Response = dt.Rows[0][0].ToString();
                if (Response == "Success")
                {
                    return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                }
                else
                {
                    if (Response.Contains("UQ__tblChari__4849DA01906D6338") == true)
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Phone No is already taken" });
                    }
                    else if (Response.Contains("UQ__tblChari__AB6E6164E0E77029") == true)
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Email is already taken" });
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = Response });
                    }
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateCharity", e.Message);
                if (e.Message.Contains("UQ__tblChari__4849DA01906D6338") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Phone No is already taken" });
                }
                if (e.Message.Contains("UQ__tblChari__AB6E6164E0E77029") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Email is already taken" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
                }
            }
        }
        #endregion

        #region deleteCharity
        /// <summary>
        /// To deleteCharity
        /// </summary>
        [HttpDelete, Route("deleteCharity")]
        public IActionResult deleteCharity([Required]int charityId)
        {
            try
            {
                    DataTable dt = Data.Charity.deleteCharity(charityId);
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
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("deleteCharity", e.Message.ToString());

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion

        #region listCharity
        /// <summary>
        /// To listCharity
        /// </summary>
        [HttpGet, Route("listCharity")]
        public IActionResult listCharity(string SearchTerm)
        {
            List<dynamic> listCharity = new List<dynamic>();

            try
            {
                DataTable dt = Data.Charity.listCharity(SearchTerm);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic Charity = new System.Dynamic.ExpandoObject();
                        Charity.charityId = (int)dt.Rows[i]["charityId"];
                        Charity.name = (dt.Rows[i]["name"] == DBNull.Value ? "" : dt.Rows[i]["name"].ToString());
                        Charity.phoneNumber = (dt.Rows[i]["phoneNumber"] == DBNull.Value ? "" : dt.Rows[i]["phoneNumber"].ToString());
                        Charity.email = (dt.Rows[i]["email"] == DBNull.Value ? "" : dt.Rows[i]["email"].ToString());
                        Charity.gender = (dt.Rows[i]["gender"] == DBNull.Value ? "" : dt.Rows[i]["gender"].ToString());
                        Charity.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        Charity.profileImage = (dt.Rows[i]["profileImage"] == DBNull.Value ? "" : dt.Rows[i]["profileImage"].ToString());
                        Charity.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        //Charity.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());

                        listCharity.Add(Charity);
                    }
                    return StatusCode((int)HttpStatusCode.OK, listCharity);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, listCharity);
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listCharity", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region selectByCharityId
        /// <summary>
        /// To  selectByCharityId
        /// </summary>
        [HttpGet, Route("selectByCharityId")]
        public IActionResult selectByCharityId([Required]int charityId)
        {

            try
            {
                DataTable dt = Data.Charity.selectByCharityId(charityId);

                dynamic Charity = new System.Dynamic.ExpandoObject();
                if (dt.Rows.Count > 0)
                {
                    Charity.charityId = (int)dt.Rows[0]["charityId"];
                    Charity.firstName = (dt.Rows[0]["firstName"] == DBNull.Value ? "" : dt.Rows[0]["firstName"].ToString());
                    Charity.lastName = (dt.Rows[0]["lastName"] == DBNull.Value ? "" : dt.Rows[0]["lastName"].ToString());
                    Charity.phoneNumber = (dt.Rows[0]["phoneNumber"] == DBNull.Value ? "" : dt.Rows[0]["phoneNumber"].ToString());
                    Charity.email = (dt.Rows[0]["email"] == DBNull.Value ? "" : dt.Rows[0]["email"].ToString());
                    Charity.gender = (dt.Rows[0]["gender"] == DBNull.Value ? "" : dt.Rows[0]["gender"].ToString());
                    Charity.address = (dt.Rows[0]["address"] == DBNull.Value ? "" : dt.Rows[0]["address"].ToString());
                    Charity.profileImage = (dt.Rows[0]["profileImage"] == DBNull.Value ? "" : dt.Rows[0]["profileImage"].ToString());
                    Charity.createdDate = (dt.Rows[0]["createdDate"] == DBNull.Value ? "" : dt.Rows[0]["createdDate"].ToString());

                    return StatusCode((int)HttpStatusCode.OK, Charity);
                }

                else
                {
                    return StatusCode((int)HttpStatusCode.OK, Charity);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("selectByCharityId", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion
    }
}