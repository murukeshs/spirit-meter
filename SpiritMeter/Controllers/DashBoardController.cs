using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SpiritMeter.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DashBoardController : ControllerBase
    {
        #region dashBoardData
        /// <summary>
        /// To list dashBoard data
        /// </summary>
        [HttpGet, Route("dashBoardData")]
        public IActionResult dashBoardData()
        {
           
            dynamic data = new System.Dynamic.ExpandoObject();
            List<dynamic> listDisplay= new List<dynamic>();
            List<dynamic> listRoutes = new List<dynamic>();
            List<dynamic> popularDisplay = new List<dynamic>();
            try
            {
                DataSet ds = Data.User.dashBoardData();
                
                if (ds.Tables[0].Rows.Count > 0)
                {

                    data.userCount = (int)ds.Tables[0].Rows[0]["userCount"];
                    data.savedDisplays = (int)ds.Tables[1].Rows[0]["savedDisplays"];
                    data.savedRoutes = (int)ds.Tables[2].Rows[0]["savedRoutes"];
                    
                    for (int i = 0; i < ds.Tables[3].Rows.Count; i++)
                    {
                        dynamic display = new System.Dynamic.ExpandoObject();
                        display.name = (ds.Tables[3].Rows[i]["name"] == DBNull.Value ? "" : ds.Tables[3].Rows[i]["name"].ToString());
                        display.savedDisplays = (int)ds.Tables[3].Rows[i]["savedDisplays"];
                        listDisplay.Add(display);
                    }
                    data.userWithDisplay = listDisplay;
                    
                    for (int i = 0; i < ds.Tables[4].Rows.Count; i++)
                    {
                        dynamic routes = new System.Dynamic.ExpandoObject();
                        routes.name = (ds.Tables[4].Rows[i]["name"] == DBNull.Value ? "" : ds.Tables[4].Rows[i]["name"].ToString());
                        routes.savedRoutes = (int)ds.Tables[4].Rows[i]["savedRoutes"];
                        listRoutes.Add(routes);
                    }
                    data.userWithRoutes = listRoutes;
                   
                    for (int i = 0; i < ds.Tables[5].Rows.Count; i++)
                    {
                        dynamic populardis = new System.Dynamic.ExpandoObject();
                        populardis.displayId = (int)ds.Tables[5].Rows[i]["displayId"];
                        populardis.displayName = (ds.Tables[5].Rows[i]["name"] == DBNull.Value ? "" : ds.Tables[5].Rows[i]["name"].ToString());
                        populardis.charityName = (ds.Tables[5].Rows[i]["charityName"] == DBNull.Value ? "" : ds.Tables[5].Rows[i]["charityName"].ToString());
                        populardis.count = (int)ds.Tables[5].Rows[i]["counts"];
                        popularDisplay.Add(populardis);
                    }
                    data.popularDisplay = popularDisplay;
                    //listData.Add(data);
                    return StatusCode((int)HttpStatusCode.OK, data);
                }
             
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, data);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("dashBoardData", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

    }
}