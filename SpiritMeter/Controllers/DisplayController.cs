﻿using System;
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
    [Authorize]
    public class DisplayController : ControllerBase
    {
        #region listCategory
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
                else if (createDisplay.categoryId <= 0 || createDisplay.categoryId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter categoryId" });
                }
                
                else if (createDisplay.type == "" || createDisplay.type == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter type" });
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
                        display.categoryId = (ds.Tables[0].Rows[0]["categoryId"] == DBNull.Value ? 0 : (int)ds.Tables[0].Rows[0]["categoryId"]);
                        display.categoryName = (ds.Tables[0].Rows[0]["categoryName"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["categoryName"].ToString());
                        display.notes = (ds.Tables[0].Rows[0]["notes"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["notes"].ToString());
                        display.latitude = (ds.Tables[0].Rows[0]["latitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["latitude"].ToString());
                        display.longitude = (ds.Tables[0].Rows[0]["longitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["longitude"].ToString());
                        display.country = (ds.Tables[0].Rows[0]["country"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["country"].ToString());
                        display.state = (ds.Tables[0].Rows[0]["state"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["state"].ToString());
                        display.cityName = (ds.Tables[0].Rows[0]["cityName"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["cityName"].ToString());
                        display.address = (ds.Tables[0].Rows[0]["address"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["address"].ToString());
                        display.type = (ds.Tables[0].Rows[0]["type"] == DBNull.Value ?"": ds.Tables[0].Rows[0]["type"].ToString());
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
                        display.routes = (ds.Tables[0].Rows[0]["routes"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["routes"].ToString());



                    return StatusCode((int)HttpStatusCode.OK, display);  
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, "No Records Found");
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
                        listDisplay.categoryId = (dt.Rows[i]["categoryId"] == DBNull.Value ? "" : dt.Rows[i]["categoryId"].ToString());
                        listDisplay.categoryName = (dt.Rows[i]["categoryName"] == DBNull.Value ? "" : dt.Rows[i]["categoryName"].ToString());
                        listDisplay.notes = (dt.Rows[i]["notes"] == DBNull.Value ? "" : dt.Rows[i]["notes"].ToString());
                        listDisplay.latitude = (dt.Rows[i]["latitude"] == DBNull.Value ? "" : dt.Rows[i]["latitude"].ToString());   
                        listDisplay.longitude = (dt.Rows[i]["longitude"] == DBNull.Value ? "" : dt.Rows[i]["longitude"].ToString());
                        listDisplay.country = (dt.Rows[i]["country"] == DBNull.Value ? "" : dt.Rows[i]["country"].ToString());
                        listDisplay.state = (dt.Rows[i]["state"] == DBNull.Value ? "" : dt.Rows[i]["state"].ToString());
                        listDisplay.cityName = (dt.Rows[i]["cityName"] == DBNull.Value ? "" : dt.Rows[i]["cityName"].ToString());
                        listDisplay.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        listDisplay.type = (dt.Rows[i]["type"] == DBNull.Value ? "" : dt.Rows[i]["type"].ToString());
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
                        listDisplay.categoryId = (dt.Rows[i]["categoryId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["categoryId"]);
                        listDisplay.categoryName = (dt.Rows[i]["categoryName"] == DBNull.Value ? "" : dt.Rows[i]["categoryName"].ToString());
                        listDisplay.notes = (dt.Rows[i]["notes"] == DBNull.Value ? "" : dt.Rows[i]["notes"].ToString());
                        listDisplay.latitude = (dt.Rows[i]["latitude"] == DBNull.Value ? "" : dt.Rows[i]["latitude"].ToString());
                        listDisplay.longitude = (dt.Rows[i]["longitude"] == DBNull.Value ? "" : dt.Rows[i]["longitude"].ToString());
                        listDisplay.country = (dt.Rows[i]["country"] == DBNull.Value ? "" : dt.Rows[i]["country"].ToString());
                        listDisplay.state = (dt.Rows[i]["state"] == DBNull.Value ? "" : dt.Rows[i]["state"].ToString());
                        listDisplay.cityName = (dt.Rows[i]["cityName"] == DBNull.Value ? "" : dt.Rows[i]["cityName"].ToString());
                        listDisplay.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        listDisplay.type = (dt.Rows[i]["type"] == DBNull.Value ? "" : dt.Rows[i]["type"].ToString());
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
                    return StatusCode((int)HttpStatusCode.OK, "No Records Found");
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
        [HttpGet, Route("popularDisplay")]
        public IActionResult popularDisplay()
        {
            List<dynamic> listDisplayDetails = new List<dynamic>();

            try
            {
                DataTable dt = Data.Display.PopularDisplay();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic listDisplay = new System.Dynamic.ExpandoObject();
                        listDisplay.displayId = (int)dt.Rows[i]["displayId"];
                        listDisplay.name = (dt.Rows[i]["name"] == DBNull.Value ? "" : dt.Rows[i]["name"].ToString());
                        listDisplay.categoryId = (dt.Rows[i]["categoryId"] == DBNull.Value ? 0: (int)dt.Rows[i]["categoryId"]);
                        listDisplay.categoryName = (dt.Rows[i]["categoryName"] == DBNull.Value ? "" : dt.Rows[i]["categoryName"].ToString());
                        listDisplay.notes = (dt.Rows[i]["notes"] == DBNull.Value ? "" : dt.Rows[i]["notes"].ToString());
                        listDisplay.latitude = (dt.Rows[i]["latitude"] == DBNull.Value ? "" : dt.Rows[i]["latitude"].ToString());
                        listDisplay.longitude = (dt.Rows[i]["longitude"] == DBNull.Value ? "" : dt.Rows[i]["longitude"].ToString());
                        listDisplay.country = (dt.Rows[i]["country"] == DBNull.Value ? "" : dt.Rows[i]["country"].ToString());
                        listDisplay.state = (dt.Rows[i]["state"] == DBNull.Value ? "" : dt.Rows[i]["state"].ToString());
                        listDisplay.cityName = (dt.Rows[i]["cityName"] == DBNull.Value ? "" : dt.Rows[i]["cityName"].ToString());
                        listDisplay.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        listDisplay.type = (dt.Rows[i]["type"] == DBNull.Value ? "" : dt.Rows[i]["type"].ToString());
                        listDisplay.isPrivate = (dt.Rows[i]["isPrivate"] == DBNull.Value ? false : (bool)dt.Rows[i]["isPrivate"]);
                        listDisplay.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        listDisplay.createdBy = (dt.Rows[i]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[i]["createdBy"]);
                        listDisplay.createdByName = (dt.Rows[i]["createdByName"] == DBNull.Value ? "" : dt.Rows[i]["createdByName"].ToString());
                        listDisplay.routes = (dt.Rows[i]["routes"] == DBNull.Value ? "" : dt.Rows[i]["routes"].ToString());
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
    }

}