using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static SpiritMeter.Models.AdsModule;

namespace SpiritMeter.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AdsController : ControllerBase
    {

        #region createAd  
        /// <summary>
        /// To createAd
        /// </summary>
        [HttpPost, Route("createAd")]
        public IActionResult createAd(CreateAd createAd)
        {
            try
            {
                if (String.IsNullOrEmpty(createAd.name))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter name" });
                }
              
                else if (String.IsNullOrEmpty(createAd.priority) )
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter priority" });
                }

                DataTable dt = Data.Ads.createAd(createAd);


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
                string SaveErrorLog = Data.Common.SaveErrorLog("createAd", e.Message);
                
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
               
            }
        }
        #endregion 

        #region updateAd
        /// <summary>
        /// To updateAd
        /// </summary>
        [HttpPut, Route("updateAd")]
        public IActionResult updateAd([FromBody]UpdateAd updateAd )
        {
            try
            {
                
                DataTable dt = Data.Ads.updateAd(updateAd);
                string Response = dt.Rows[0][0].ToString();
                if (Response == "Success")
                {
                    return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                }
                else
                {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = Response });
                   
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateCharity", e.Message);
               
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region deleteAd
        /// <summary>
        /// To deleteAd
        /// </summary>
        [HttpDelete, Route("deleteAd")]
        public IActionResult deleteCharity([Required]int adId)
        {
            try
            {
                DataTable dt = Data.Ads.deleteAd(adId);
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
                string SaveErrorLog = Data.Common.SaveErrorLog("deleteAd", e.Message.ToString());

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion

        #region listAd
        /// <summary>
        /// To listAd
        /// </summary>
        [HttpGet, Route("listAd")]
        public IActionResult listAd(string SearchTerm)
        {
            List<dynamic> listAd = new List<dynamic>();

            try
            {
                DataTable dt = Data.Ads.listAd(SearchTerm);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic Ad = new System.Dynamic.ExpandoObject();
                        Ad.adId = (int)dt.Rows[i]["adId"];
                        Ad.name = (dt.Rows[i]["name"] == DBNull.Value ? "" : dt.Rows[i]["name"].ToString());
                        Ad.description = (dt.Rows[i]["description"] == DBNull.Value ? "" : dt.Rows[i]["description"].ToString());
                        Ad.navigationURL = (dt.Rows[i]["navigatioURL"] == DBNull.Value ? "" : dt.Rows[i]["navigatioURL"].ToString());
                        Ad.image = (dt.Rows[i]["image"] == DBNull.Value ? "" : dt.Rows[i]["image"].ToString());
                        Ad.priority = (dt.Rows[i]["priority"] == DBNull.Value ? "" : dt.Rows[i]["priority"].ToString());
                        Ad.expiryDate = (dt.Rows[i]["expiryDate"] == DBNull.Value ? "" : dt.Rows[i]["expiryDate"].ToString());
                        Ad.createddate = (dt.Rows[i]["createddate"] == DBNull.Value ? "" : dt.Rows[i]["createddate"].ToString());
                        Ad.publishedDate = (dt.Rows[i]["publishedDate"] == DBNull.Value ? "" : dt.Rows[i]["publishedDate"].ToString());
                        Ad.adStatus = (dt.Rows[i]["adStatus"] == DBNull.Value ? false : (bool)dt.Rows[i]["adStatus"]);
                        listAd.Add(Ad);
                    }
                    return StatusCode((int)HttpStatusCode.OK, listAd);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, listAd);
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listAd  ", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region selectadByAd
        /// <summary>
        /// To  selectAdById
        /// </summary>
        [HttpGet, Route("selectAdById")]
        public IActionResult selectAdById([Required]int adId)
        {

            try
            {
                DataTable dt = Data.Ads.selectAdById(adId);

                dynamic Ad = new System.Dynamic.ExpandoObject();
                if (dt.Rows.Count > 0)
                {
                    Ad.adId = (int)dt.Rows[0]["adId"];
                    Ad.name = (dt.Rows[0]["name"] == DBNull.Value ? "" : dt.Rows[0]["name"].ToString());
                    Ad.description = (dt.Rows[0]["description"] == DBNull.Value ? "" : dt.Rows[0]["description"].ToString());
                    Ad.navigationURL = (dt.Rows[0]["navigatioURL"] == DBNull.Value ? "" : dt.Rows[0]["navigatioURL"].ToString());
                    Ad.image = (dt.Rows[0]["image"] == DBNull.Value ? "" : dt.Rows[0]["image"].ToString());
                    Ad.priority = (dt.Rows[0]["priority"] == DBNull.Value ? "" : dt.Rows[0]["priority"].ToString());
                    Ad.expiryDate = (dt.Rows[0]["expiryDate"] == DBNull.Value ? "" : dt.Rows[0]["expiryDate"].ToString());
                    Ad.createddate = (dt.Rows[0]["createddate"] == DBNull.Value ? "" : dt.Rows[0]["createddate"].ToString());
                    Ad.publishedDate = (dt.Rows[0]["publishedDate"] == DBNull.Value ? "" : dt.Rows[0]["publishedDate"].ToString());
                    Ad.adStatus = (dt.Rows[0]["adStatus"] == DBNull.Value ? false : (bool)dt.Rows[0]["adStatus"]);

                    return StatusCode((int)HttpStatusCode.OK, Ad);
                }

                else
                {
                    return StatusCode((int)HttpStatusCode.OK, Ad);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("selectadByAd", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

    }
}