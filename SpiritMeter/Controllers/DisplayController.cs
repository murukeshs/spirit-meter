using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiritMeter.Data;
using SpiritMeter.Models;

namespace SpiritMeter.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DisplayController : ControllerBase
    {
        #region listCategory
        /// <summary>
        /// To list category
        /// </summary>
        [HttpGet, Route("listCategory")]
        public IActionResult listCategory()
        {
            List<dynamic> listCategoryDetails = new List<dynamic>();

            try
            {
                DataTable dt = Data.Display.listCategory();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic listCategory = new System.Dynamic.ExpandoObject();
                        listCategory.categoryId = (int)dt.Rows[i]["categoryId"];
                        listCategory.categoryName = (dt.Rows[i]["categoryName"] == DBNull.Value ? "" : dt.Rows[i]["categoryName"].ToString());
                        listCategory.isDeleted = (dt.Rows[i]["isDeleted"] == DBNull.Value ? false : (bool)dt.Rows[i]["isDeleted"]);

                        listCategoryDetails.Add(listCategory);
                    }
                    return StatusCode((int)HttpStatusCode.OK, listCategoryDetails);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, listCategoryDetails);
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listUser", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion



        #region createDisplay
        /// <summary>
        /// To createDisplay
        /// </summary>
        [HttpPost, Route("createDisplay")]
        public IActionResult createDisplay(createDisplay createDisplay)
        {
            try
            {
                List<createDisplay> displayList = new List<createDisplay>();

                if (createDisplay.name == "" || createDisplay.name == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter  Name" });
                }
               
                else if (createDisplay.markerType == "" || createDisplay.markerType == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter markerType" });
                }
                else if (createDisplay.createdBy <=0 ||  createDisplay.createdBy == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter createdBy" });
                }

                DataSet ds = Data.Display.createDisplay(createDisplay);
                string Response = ds.Tables[0].Rows[0]["SuccessMessage"].ToString();
               
                if (Response == "Success")
                {
                    string displayId = ds.Tables[1].Rows[0]["displayId"].ToString();
                    return StatusCode((int)HttpStatusCode.OK, new { displayId, message = "Display Created successfully" });
                }

                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Error while creating the Display" });
                }
            }
            catch (Exception e)
            {
                    string SaveErrorLog = Data.Common.SaveErrorLog("createDisplay", e.Message);
                if (e.Message.Contains("UQ__tblDispl__E0DD8006C302141D") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Display already created" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
                }
                
            }
        }

        #endregion
        #region createDisplayFiles
        /// <summary>
        /// to createDisplayFiles
        /// </summary>
        [HttpPost, Route("createDisplayFiles")]
        public IActionResult createDisplayFiles(createDisplayFiles createDisplayFiles)
        {
            try
            {
                List<createDisplay> displayList = new List<createDisplay>();

                if (createDisplayFiles.displayId <= 0 || createDisplayFiles.displayId== null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter displayId" });
                }
                else if(createDisplayFiles.filePath == "" || createDisplayFiles.filePath == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please upload filePath" });
                }

                int row = Data.Display.createDisplayFiles(createDisplayFiles);
                if (row > 0)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Created DisplayFiles Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Error while Updating the DisplayFiles" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("createDisplayFiles", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });

            }
        }

        #endregion


        #region updateDisplay
        /// <summary>
        /// To updateDisplay
        /// </summary>
        [HttpPut, Route("updateDisplay")]
        public IActionResult updateDisplay(updateDisplay createDisplay)
        {
            try
            {
                List<createDisplay> displayList = new List<createDisplay>();

                
                if (createDisplay.displayId <= 0 || createDisplay.displayId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter displayId" });
                }
               


                int row = Data.Display.updateDisplay(createDisplay);
                if (row > 0)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Error while Updating the Display" });
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateDisplay", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });

            }
        }

        #endregion

        #region deleteDisplayFiles
        /// <summary>
        /// To deleteDisplayFiles
        /// </summary>
        [HttpDelete, Route("deleteDisplayFiles")]
        public IActionResult deleteDisplayFiles(int displayFileId)
        {
            try
            {
                if (displayFileId <= 0 || displayFileId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter displayFileId" });
                }
                else
                {

                    int row = Data.Display.deleteDisplayFiles(displayFileId);
                    if (row > 0)
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Deleted Successfully");
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Error while Deleting the DisplayFiles" });
                    }
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("deleteDisplayFiles", e.Message.ToString());

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion

        #region selectDisplay
        /// <summary>
        /// To selectDisplay by displayid
        /// </summary>
        [HttpGet, Route("selectDisplay/{displayId}")]
        public IActionResult selectDisplay(int displayId)
        {
          

            try
            {
                DataSet ds = Data.Display.selectDisplay(displayId);
                dynamic display = new System.Dynamic.ExpandoObject();
                if (ds.Tables[0].Rows.Count > 0)
                {  
                        display.displayId = (int)ds.Tables[0].Rows[0]["displayId"];
                        display.name = (ds.Tables[0].Rows[0]["name"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["name"].ToString());
                        display.charityId =(ds.Tables[0].Rows[0]["charityId"] == DBNull.Value ? 0 : (int)ds.Tables[0].Rows[0]["charityId"]);
                        display.charityName = (ds.Tables[0].Rows[0]["charityName"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["charityName"].ToString());
                        display.notes = (ds.Tables[0].Rows[0]["notes"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["notes"].ToString());
                        display.latitude = (ds.Tables[0].Rows[0]["latitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["latitude"].ToString());
                        display.longitude = (ds.Tables[0].Rows[0]["longitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["longitude"].ToString());
                        display.country = (ds.Tables[0].Rows[0]["country"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["country"].ToString());
                        display.state = (ds.Tables[0].Rows[0]["state"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["state"].ToString());
                        display.cityName = (ds.Tables[0].Rows[0]["cityName"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["cityName"].ToString());
                        display.address = (ds.Tables[0].Rows[0]["address"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["address"].ToString());
                        display.markerType = (ds.Tables[0].Rows[0]["markerType"] == DBNull.Value ?"": ds.Tables[0].Rows[0]["markerType"].ToString());
                        display.isPrivate = (ds.Tables[0].Rows[0]["isPrivate"] == DBNull.Value ? false : (bool)ds.Tables[0].Rows[0]["isPrivate"]);
                        display.createdDate = (ds.Tables[0].Rows[0]["createdDate"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["createdDate"].ToString());
                        display.createdBy = (ds.Tables[0].Rows[0]["createdBy"] == DBNull.Value ? 0 : (int)ds.Tables[0].Rows[0]["createdBy"]);
                        display.createdByName = (ds.Tables[0].Rows[0]["createdByName"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["createdByName"].ToString());
                        
                        List<dynamic> listFilePaths = new List<dynamic>();
                       
                        for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                        {
                            dynamic filePath = new System.Dynamic.ExpandoObject();
                            filePath.displayFileId = (ds.Tables[1].Rows[j]["displayFileId"] == DBNull.Value ? 0 : (int)ds.Tables[1].Rows[j]["displayFileId"]);
                            filePath.FilePath = (ds.Tables[1].Rows[j]["filePath"] == DBNull.Value ? "" : ds.Tables[1].Rows[j]["filePath"].ToString());
                            listFilePaths.Add(filePath);
                        }
                        display.filePath = listFilePaths;
                    //display.path = (ds.Tables[0].Rows[0]["path"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["path"].ToString());
                    display.routes = (ds.Tables[0].Rows[0]["routes"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["routes"].ToString());



                    return StatusCode((int)HttpStatusCode.OK, display);  
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, display);
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("selectDisplay", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region deleteDisplay
        /// <summary>
        /// To deleteDisplay by displayid
        /// </summary>
        [HttpDelete, Route("deleteDisplay")]
        public IActionResult deleteDisplay(int displayId)
        {
            try
            {
                if (displayId <= 0 || displayId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter displayId" });
                }
                else
                {

                    int row = Data.Display.deleteDisplay(displayId);
                    if (row > 0)
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Deleted Successfully");
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Error while Deleting the Display" });
                    }
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("deleteDisplay", e.Message.ToString());

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion

        #region listDisplay
        /// <summary>
        /// To listDisplay
        /// </summary>
        [HttpGet, Route("listDisplay")]
        public IActionResult listDisplay(string Search)
        {
            List<dynamic> listDisplayDetails = new List<dynamic>();

            try
            {
                DataTable dt = Data.Display.listDisplay(Search);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic listDisplay = new System.Dynamic.ExpandoObject();
                        listDisplay.displayId = (int)dt.Rows[i]["displayId"];
                        listDisplay.name = (dt.Rows[i]["name"] == DBNull.Value ? "" : dt.Rows[i]["name"].ToString());
                        listDisplay.charityId = (dt.Rows[i]["charityId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["charityId"]); 
                        listDisplay.charityName = (dt.Rows[i]["charityName"] == DBNull.Value ? "" : dt.Rows[i]["charityName"].ToString());
                        listDisplay.notes = (dt.Rows[i]["notes"] == DBNull.Value ? "" : dt.Rows[i]["notes"].ToString());
                        listDisplay.latitude = (dt.Rows[i]["latitude"] == DBNull.Value ? "" : dt.Rows[i]["latitude"].ToString());   
                        listDisplay.longitude = (dt.Rows[i]["longitude"] == DBNull.Value ? "" : dt.Rows[i]["longitude"].ToString());
                        listDisplay.country = (dt.Rows[i]["country"] == DBNull.Value ? "" : dt.Rows[i]["country"].ToString());
                        listDisplay.state = (dt.Rows[i]["state"] == DBNull.Value ? "" : dt.Rows[i]["state"].ToString());
                        listDisplay.cityName = (dt.Rows[i]["cityName"] == DBNull.Value ? "" : dt.Rows[i]["cityName"].ToString());
                        listDisplay.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        listDisplay.markerType = (dt.Rows[i]["markerType"] == DBNull.Value ? "" : dt.Rows[i]["markerType"].ToString());
                        listDisplay.viewCount    = (dt.Rows[i]["viewCount"] == DBNull.Value ? 0 : (int)dt.Rows[i]["viewCount"]);
                        listDisplay.isPrivate = (dt.Rows[i]["isPrivate"] == DBNull.Value ? false :(bool) dt.Rows[i]["isPrivate"]);
                        listDisplay.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        listDisplay.createdBy = (dt.Rows[i]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[i]["createdBy"]);
                        listDisplay.createdByName = (dt.Rows[i]["createdName"] == DBNull.Value ? "" : dt.Rows[i]["createdName"].ToString());
                        listDisplay.filePath = (dt.Rows[i]["filePath"] == DBNull.Value ? "" : dt.Rows[i]["filePath"].ToString());

                        listDisplayDetails.Add(listDisplay);

                    }
                    return StatusCode((int)HttpStatusCode.OK, listDisplayDetails);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, listDisplayDetails);
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listDisplay", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion
        #region listDisplayByUserId
        /// <summary>
        /// To listDisplayByUserId 
        /// </summary>
        [HttpGet, Route("listDisplayByUserId/{userId}")]
        public IActionResult listDisplayByUserId(int userId)
        {
            List<dynamic> listDisplayDetails = new List<dynamic>();

            try
            {
                DataTable dt = Data.Display.listDisplayByUserId(userId);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic listDisplay = new System.Dynamic.ExpandoObject();
                        listDisplay.displayId = (int)dt.Rows[i]["displayId"];
                        listDisplay.name = (dt.Rows[i]["name"] == DBNull.Value ? "" : dt.Rows[i]["name"].ToString());
                        listDisplay.charityId = (dt.Rows[i]["charityId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["charityId"]);
                        listDisplay.charityName = (dt.Rows[i]["charityName"] == DBNull.Value ? "" : dt.Rows[i]["charityName"].ToString());
                        listDisplay.notes = (dt.Rows[i]["notes"] == DBNull.Value ? "" : dt.Rows[i]["notes"].ToString());
                        listDisplay.latitude = (dt.Rows[i]["latitude"] == DBNull.Value ? "" : dt.Rows[i]["latitude"].ToString());
                        listDisplay.longitude = (dt.Rows[i]["longitude"] == DBNull.Value ? "" : dt.Rows[i]["longitude"].ToString());
                        listDisplay.country = (dt.Rows[i]["country"] == DBNull.Value ? "" : dt.Rows[i]["country"].ToString());
                        listDisplay.state = (dt.Rows[i]["state"] == DBNull.Value ? "" : dt.Rows[i]["state"].ToString());
                        listDisplay.cityName = (dt.Rows[i]["cityName"] == DBNull.Value ? "" : dt.Rows[i]["cityName"].ToString());
                        listDisplay.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        listDisplay.markerType = (dt.Rows[i]["markerType"] == DBNull.Value ? "" : dt.Rows[i]["markerType"].ToString());
                        listDisplay.viewCount = (dt.Rows[i]["viewCount"] == DBNull.Value ? 0 : (int)dt.Rows[i]["viewCount"]);
                        listDisplay.isPrivate = (dt.Rows[i]["isPrivate"] == DBNull.Value ? false : (bool)dt.Rows[i]["isPrivate"]);
                        listDisplay.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        listDisplay.createdBy = (dt.Rows[i]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[i]["createdBy"]);
                        listDisplay.createdByName = (dt.Rows[i]["createdName"] == DBNull.Value ? "" : dt.Rows[i]["createdName"].ToString());
                        listDisplay.filePath = (dt.Rows[i]["filePath"] == DBNull.Value ? "" : dt.Rows[i]["filePath"].ToString());
                        listDisplayDetails.Add(listDisplay);
                    }
                    return StatusCode((int)HttpStatusCode.OK, listDisplayDetails);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, listDisplayDetails);
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listDisplayByUserId", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion


        #region popularDisplay
        /// <summary>
        /// To display popularDisplays
        /// </summary>
        [HttpGet, Route("popularDisplay")]        
        public IActionResult popularDisplay(int userId)
        {
            List<dynamic> listDisplayDetails = new List<dynamic>();

            try
            {
                DataTable dt = Data.Display.PopularDisplay(userId);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic listDisplay = new System.Dynamic.ExpandoObject();
                        listDisplay.displayId = (int)dt.Rows[i]["displayId"];
                        listDisplay.name = (dt.Rows[i]["name"] == DBNull.Value ? "" : dt.Rows[i]["name"].ToString());
                        listDisplay.notes = (dt.Rows[i]["notes"] == DBNull.Value ? "" : dt.Rows[i]["notes"].ToString());
                        listDisplay.charityId = (dt.Rows[i]["charityId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["charityId"]); 
                        listDisplay.charityName = (dt.Rows[i]["charityName"] == DBNull.Value ? "" : dt.Rows[i]["charityName"].ToString());
                        listDisplay.latitude = (dt.Rows[i]["latitude"] == DBNull.Value ? "" : dt.Rows[i]["latitude"].ToString());
                        listDisplay.longitude = (dt.Rows[i]["longitude"] == DBNull.Value ? "" : dt.Rows[i]["longitude"].ToString());
                        listDisplay.country = (dt.Rows[i]["country"] == DBNull.Value ? "" : dt.Rows[i]["country"].ToString());
                        listDisplay.state = (dt.Rows[i]["state"] == DBNull.Value ? "" : dt.Rows[i]["state"].ToString());
                        listDisplay.cityName = (dt.Rows[i]["cityName"] == DBNull.Value ? "" : dt.Rows[i]["cityName"].ToString());
                        listDisplay.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        listDisplay.markerType = (dt.Rows[i]["markerType"] == DBNull.Value ? "" : dt.Rows[i]["markerType"].ToString());
                        listDisplay.isPrivate = (dt.Rows[i]["isPrivate"] == DBNull.Value ? false : (bool)dt.Rows[i]["isPrivate"]);
                        listDisplay.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        listDisplay.createdBy = (dt.Rows[i]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[i]["createdBy"]);
                        listDisplay.createdByName = (dt.Rows[i]["createdByName"] == DBNull.Value ? "" : dt.Rows[i]["createdByName"].ToString());
                        listDisplay.filePath = (dt.Rows[i]["filePath"] == DBNull.Value ? "" : dt.Rows[i]["filePath"].ToString());
                        //listDisplay.routes = (dt.Rows[i]["routes"] == DBNull.Value ? "" : dt.Rows[i]["routes"].ToString());
                        listDisplayDetails.Add(listDisplay);
                    }
                    return StatusCode((int)HttpStatusCode.OK, listDisplayDetails);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, listDisplayDetails);
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("popularDisplay", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion


        #region createDisplayCharity  
        /// <summary>
        /// To createDisplayCharity
        /// </summary>
        [HttpPost, Route("createDisplayCharity")]
        public IActionResult createDisplayCharity(CreatelDisplayCharity createDisplayCharity)
        {
            try
            {
                if (createDisplayCharity.displayId == 0 && createDisplayCharity.displayId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter displayId" });
                }
                else if (createDisplayCharity.charityId == 0 && createDisplayCharity.charityId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter charityId" });
                }
            
                DataTable dt = Data.Display.createDisplayCharity(createDisplayCharity);


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
                string SaveErrorLog = Data.Common.SaveErrorLog("createDisplayCharity", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });

            }
        }
        #endregion 

        #region updateDisplayCharity
        /// <summary>
        /// To updateDisplayCharity
        /// </summary>
        [HttpPut, Route("updateDisplayCharity")]
        public IActionResult updateDisplayCharity([FromBody]UpdateDisplayCharity updateDisplayCharity)
        {
            try
            {

                DataTable dt = Data.Display.updateDisplayCharity(updateDisplayCharity);
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
                string SaveErrorLog = Data.Common.SaveErrorLog("updateDisplayCharity", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion
    }

}