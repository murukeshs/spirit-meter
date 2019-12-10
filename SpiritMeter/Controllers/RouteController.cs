﻿using System;
using System.Collections.Generic;
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

namespace SpiritMeter.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class RouteController : ControllerBase
    {
        #region createRoute
        [HttpPost, Route("createRoute")]
        public IActionResult createRoute(createRoute createRoute)
        {
            try
            {
                if (createRoute.routeName == "" || createRoute.routeName == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter routeName" });
                }
                else if (createRoute.designatedCharityId < 0 || createRoute.designatedCharityId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter userId" });
                }
               
                DataSet ds = Data.Route.createRoute(createRoute);
                string Response = ds.Tables[0].Rows[0]["SuccessMessage"].ToString();

                if (Response == "Success")
                {
                    string displayId = ds.Tables[1].Rows[0]["routeId"].ToString();
                    return StatusCode((int)HttpStatusCode.OK, new { displayId, message = "Route Created successfully" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = Response });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("createRoute", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
               
            }
        }
        #endregion

        #region updateRoute
        [HttpPut, Route("updateRoute")]
        public IActionResult updateRoute([FromBody]createRoute updateRoute)
        {
            try
            {
                DataTable dt = Data.Route.updateRoute(updateRoute);
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
                string SaveErrorLog = Data.Common.SaveErrorLog("updateRoute", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
                
            }
        }
        #endregion

        #region saveRoutePoints
        [HttpPost, Route("saveRoutePoints")]
        public async Task<IActionResult> saveRoutePoints(routePoints routePoints)
        {
            try
            {

                if (routePoints.routeId < 0 || routePoints.routeId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter routeId" });
                }
                else if (routePoints.displayId == "" || routePoints.displayId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter displayId" });
                }
                DataTable dt1 = Data.Route.selectcoordinates(routePoints);
                var response = "";

                using (var client = new HttpClient())
                {
                    var a = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + dt1.Rows[0][0].ToString() + "&destinations=" + dt1.Rows[0][1].ToString() + "&mode=train|tram|subway&language=fr-FR&key= " + Common.Apikey();
                    response = await client.GetStringAsync(string.Format(a));
                }
                JObject o = JObject.Parse(@response);
                List<JToken> acme = o.SelectTokens("$...['elements'].['distance'].['value']").ToList();
                var position = acme.IndexOf(acme.Max());

                //dynamic obj = JsonConvert.DeserializeObject(response);

                //List<decimal> list = new List<decimal>();
                //for (var i=0;i< (obj.rows[0].elements).Count; i++)
                //{
                    
                //    string a = obj.rows[0].elements[i].distance.text;
                //    decimal b = Decimal.Parse(a.Remove(a.Length - 3).Replace(",", "."));
                //    list.Add(b);
                //}
                //var position= list.IndexOf(list.Max());
                List<string> numbers = (dt1.Rows[0][1].ToString()).Split('|').ToList<string>();
                string destination = numbers[position];
                 numbers.RemoveAt(position);
                using (var client = new HttpClient())   
                {
                    var a = "https://maps.googleapis.com/maps/api/directions/json?origin=" + dt1.Rows[0][0].ToString() + "&destination=" + destination + "&waypoints=optimize:true|" + string.Join<string>("|", numbers) + "&key=" + Common.Apikey();
                    string path = await client.GetStringAsync(string.Format(a));

                DataTable dt = Data.Route.saveRoutePoints(routePoints, path);

                string Response = dt.Rows[0][0].ToString();

                if (Response == "Success")
                {
                    return StatusCode((int)HttpStatusCode.OK, "RoutePoints Successfully Created");
                }
                else
                {

                    if (Response.Contains("UQ__tblRoute__179688842B0C597E") == true)
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "routeId/displayId are already taken" });
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = Response });
                    }

                }
                }

            }

            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("saveRoutePoints", e.Message);
                if (e.Message.Contains("UQ__tblRoute__179688842B0C597E") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "routeId/displayId are already taken" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
                }

            }
        }
        #endregion
        #region calculatePath
        [HttpPost, Route("calculatePath")]
        public async Task<IActionResult> calculatePath(calculatepath calculatepath)
        {
            try
            {

                routePoints routePoints = new routePoints();
                routePoints.startingPoint = calculatepath.startingPoint;
                routePoints.displayId = calculatepath.displayId;
                DataTable dt1 = Data.Route.selectcoordinates(routePoints);
                var response = "";

                using (var client = new HttpClient())
                {
                    var a = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + dt1.Rows[0][0].ToString() + "&destinations=" + dt1.Rows[0][1].ToString() + "&mode=train|tram|subway&language=fr-FR&key= " + Common.Apikey();
                    response = await client.GetStringAsync(string.Format(a));
                }

                JObject o = JObject.Parse(@response);

                List<JToken> acme = o.SelectTokens("$...['elements'].['distance'].['value']").ToList();
                var position = acme.IndexOf(acme.Max());
                //List<JToken> acme1 = o1.SelectToken("$.distance[*].value").ToList();


                //dynamic obj = JsonConvert.DeserializeObject(response);

                //List<decimal> list = new List<decimal>();
                //for (var i = 0; i < (obj.rows[0].elements).Count; i++)
                //{

                //    string a = obj.rows[0].elements[i].distance.text;
                //    decimal b = Decimal.Parse(a.Remove(a.Length - 3).Replace(",", "."));
                //    list.Add(b);
                //}
                //var position = list.IndexOf(list.Max());
                List<string> numbers = (dt1.Rows[0][1].ToString()).Split('|').ToList<string>();
                string destination = numbers[position];
                numbers.RemoveAt(position);
                using (var client = new HttpClient())
                {
                    var a = "https://maps.googleapis.com/maps/api/directions/json?origin=" + dt1.Rows[0][0].ToString() + "&destination=" + destination + "&waypoints=optimize:true|" + string.Join<string>("|", numbers) + "&key=" + Common.Apikey();
                    string path = await client.GetStringAsync(string.Format(a));
                    JObject obj = JObject.Parse(path);

                    List<JToken> acme1 = obj.SelectTokens("$..['routes'].['legs'].['steps'].['distance'].['value']").ToList();
                    int miles = acme1.Sum(x => Convert.ToInt32(x));
                    //dynamic obj1 = JsonConvert.DeserializeObject(path);

                    //int miles = 0;
                    //for (var i = 0; i < (obj1.routes[0].legs).Count; i++)
                    //{
                    //    miles = miles + (int)obj1.routes[0].legs[i].distance.value;
                    //}

                    var totalmiles = String.Concat(String.Format("{0:0.00}", (miles / 1609.344)), " mi");

                    return StatusCode((int)HttpStatusCode.OK, new { totalmiles, path, message = "Route Created successfully" });


                }

            }

            catch (Exception e)
                {
                string SaveErrorLog = Data.Common.SaveErrorLog("calculatePath", e.Message);
               
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
               
            }
        }
        #endregion
        #region deleteRoute
        [HttpDelete, Route("deleteRoute")]
        public IActionResult deleteRoute(int routeId)
        {
            try
            {
                if (routeId <= 0)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter routeId" });
                }
                else
                {
                    DataTable dt = Data.Route.deleteRoute(routeId);
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
                string SaveErrorLog = Data.Common.SaveErrorLog("deleteRoute", e.Message.ToString());

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion

        #region saveRoutePointStatus
        [HttpPost, Route("saveRoutePointStatus")]
        public IActionResult saveRoutePointStatus(routePointStatus routePointStatus)
        {
            try
            {
                if (routePointStatus.routePointId < 0 || routePointStatus.routePointId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter routePointId" });
                }
                else if (routePointStatus.userId < 0 || routePointStatus.userId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter userId" });
                }

                DataTable dt = Data.Route.saveRoutePointStatus(routePointStatus);

                string Response = dt.Rows[0][0].ToString();

                if (Response == "Success")
                {
                    return StatusCode((int)HttpStatusCode.OK, "RoutePointStatus Successfully Created");
                }
                else
                {

                    if (Response.Contains("FK__tblRouteP__displ__787EE5A0") == true)
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Invalid displayId" });
                    }
                    else if(Response.Contains("FK__tblRouteP__route__778AC167") == true)
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Invalid routeId" });
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = Response });
                    }

                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("saveRoutePointStatus", e.Message);
                if (e.Message.Contains("FK__tblRouteP__displ__787EE5A0") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Invalid displayId" });
                }
                else if(e.Message.Contains("FK__tblRouteP__route__778AC167") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Invalid routeId" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
                }

            }
        }
        #endregion

        #region saveRouteStatus
        [HttpPost, Route("saveRouteStatus")]
        public IActionResult saveRouteStatus(rideStatus rideStatus)
        {
            try
            {
                if (rideStatus.routeId < 0 || rideStatus.routeId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter routeId" });
                }
                else if (rideStatus.userId < 0 || rideStatus.userId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter userId" });
                }
                

                DataSet ds = Data.Route.saveRouteStatus(rideStatus);

                string Response = ds.Tables[0].Rows[0][0].ToString();

                if (Response == "Success" & rideStatus.status == "Start")
                {
                    string rideStatusId = ds.Tables[1].Rows[0]["rideStatusId"].ToString();
                    
                    return StatusCode((int)HttpStatusCode.OK, new { rideStatusId, message = "RouteStatus Successfully Created" });
                }
                else if (Response == "Success" & rideStatus.status == null | rideStatus.status == "")
                {
                    return StatusCode((int)HttpStatusCode.OK, new { message = "RouteStatus Successfully Updated" });
                }
                    
                else
                {

                    if (Response.Contains("FK__tblRideSt__route__02084FDA") == true)
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Invalid routeId" });
                    }
                    else if (Response.Contains("FK__tblRideSt__userI__02FC7413") == true)
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Invalid userId" });
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = Response });
                    }

                }
            }
            catch (Exception e) 
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("saveRouteStatus", e.Message);
                if (e.Message.Contains("FK__tblRideSt__route__02084FDA") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Invalid routeId" });
                }
                else if (e.Message.Contains("FK__tblRideSt__userI__02FC7413") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Invalid userId" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
                }   

            }
        }
        #endregion

        #region listRoutes
        [HttpGet, Route("listRoutes")]
        public IActionResult listRoutes(string SearchTerm)
        {
            List<dynamic> listRoutesDetails = new List<dynamic>();

            try
            {
                DataTable dt = Data.Route.listRoutes(SearchTerm);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic listRoutes = new System.Dynamic.ExpandoObject();
                        listRoutes.routeId = (int)dt.Rows[i]["routeId"];
                        listRoutes.routeName = (dt.Rows[i]["routeName"] == DBNull.Value ? "" : dt.Rows[i]["routeName"].ToString());
                        listRoutes.comments = (dt.Rows[i]["comments"] == DBNull.Value ? "" : dt.Rows[i]["comments"].ToString());
                        listRoutes.designatedCharityId = (dt.Rows[i]["designatedCharityId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["designatedCharityId"]);
                        listRoutes.designatedCharityName = (dt.Rows[i]["designatedCharityName"] == DBNull.Value ? "" : dt.Rows[i]["designatedCharityName"].ToString());
                        listRoutes.isPrivate = (dt.Rows[i]["isPrivate"] == DBNull.Value ? false : (bool)dt.Rows[i]["isPrivate"]);
                        listRoutes.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        listRoutes.startingPoint = (dt.Rows[i]["startingPoint"] == DBNull.Value ? 0 : (int)dt.Rows[i]["startingPoint"]);
                        listRoutes.latitude = (dt.Rows[i]["latitude"] == DBNull.Value ? "" : dt.Rows[i]["latitude"].ToString());
                        listRoutes.longitude = (dt.Rows[i]["longitude"] == DBNull.Value ? "" : dt.Rows[i]["longitude"].ToString());
                        listRoutes.country = (dt.Rows[i]["country"] == DBNull.Value ? "" : dt.Rows[i]["country"].ToString());
                        listRoutes.state = (dt.Rows[i]["state"] == DBNull.Value ? "" : dt.Rows[i]["state"].ToString());
                        listRoutes.cityName = (dt.Rows[i]["cityName"] == DBNull.Value ? "" : dt.Rows[i]["cityName"].ToString());
                        listRoutes.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        listRoutes.totalMiles = (dt.Rows[i]["totalMiles"] == DBNull.Value ? "" : String.Concat(dt.Rows[i]["totalMiles"], "mi").ToString());
                        listRoutes.path = (dt.Rows[i]["path"] == DBNull.Value ? "" : dt.Rows[i]["path"].ToString());
                        listRoutesDetails.Add(listRoutes);
                    }
                    return StatusCode((int)HttpStatusCode.OK, listRoutesDetails);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, listRoutesDetails);
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listRoutes", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion
        #region listRoutesByUserId
        [HttpGet, Route("listRoutesByUserId")]
        public IActionResult listRoutesByUserId(int userId)
        {
            List<dynamic> listRoutesDetails = new List<dynamic>();

            try
            {
                DataTable dt = Data.Route.listRoutesByUserId(userId);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic listRoutes = new System.Dynamic.ExpandoObject();
                        listRoutes.routeId = (int)dt.Rows[i]["routeId"];
                        listRoutes.routeName = (dt.Rows[i]["routeName"] == DBNull.Value ? "" : dt.Rows[i]["routeName"].ToString());
                        listRoutes.comments = (dt.Rows[i]["comments"] == DBNull.Value ? "" : dt.Rows[i]["comments"].ToString());
                        listRoutes.designatedCharityId = (dt.Rows[i]["designatedCharityId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["designatedCharityId"]);
                        listRoutes.designatedCharityName = (dt.Rows[i]["designatedCharityName"] == DBNull.Value ? "" : dt.Rows[i]["designatedCharityName"].ToString());
                        listRoutes.isPrivate = (dt.Rows[i]["isPrivate"] == DBNull.Value ? false : (bool)dt.Rows[i]["isPrivate"]);
                        listRoutes.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        listRoutes.startingPoint = (dt.Rows[i]["startingPoint"] == DBNull.Value ? 0 : (int)dt.Rows[i]["startingPoint"]);
                        listRoutes.latitude = (dt.Rows[i]["latitude"] == DBNull.Value ? "" : dt.Rows[i]["latitude"].ToString());
                        listRoutes.longitude = (dt.Rows[i]["longitude"] == DBNull.Value ? "" : dt.Rows[i]["longitude"].ToString());
                        listRoutes.country = (dt.Rows[i]["country"] == DBNull.Value ? "" : dt.Rows[i]["country"].ToString());
                        listRoutes.state = (dt.Rows[i]["state"] == DBNull.Value ? "" : dt.Rows[i]["state"].ToString());
                        listRoutes.cityName = (dt.Rows[i]["cityName"] == DBNull.Value ? "" : dt.Rows[i]["cityName"].ToString());
                        listRoutes.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        listRoutes.totalMiles = (dt.Rows[i]["totalMiles"] == DBNull.Value ? "" : String.Concat(dt.Rows[i]["totalMiles"], "mi").ToString());
                        listRoutes.path = (dt.Rows[i]["path"] == DBNull.Value ? "" : dt.Rows[i]["path"].ToString());
                        listRoutesDetails.Add(listRoutes);
                    }
                    return StatusCode((int)HttpStatusCode.OK, listRoutesDetails);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, "No Records Found");
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listRoutesByUserId", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion
        //#region selectRouteByUserId
        //[HttpGet, Route("selectRouteByUserId")]
        //public IActionResult selectRouteByUserId(int userId)
        //{
        //    List<dynamic> listRoutesDetails = new List<dynamic>();

        //    try
        //    {
        //        DataTable dt = Data.Route.selectRouteByUserId(userId);
        //        if (dt.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                dynamic listRoutes = new System.Dynamic.ExpandoObject();
        //                listRoutes.routeId = (int)dt.Rows[i]["routeId"];
        //                listRoutes.routeName = (dt.Rows[i]["routeName"] == DBNull.Value ? "" : dt.Rows[i]["routeName"].ToString());
        //                listRoutes.comments = (dt.Rows[i]["comments"] == DBNull.Value ? "" : dt.Rows[i]["comments"].ToString());
        //                listRoutes.designatedCharityId = (dt.Rows[i]["designatedCharityId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["designatedCharityId"]);
        //                listRoutes.designatedCharityName = (dt.Rows[i]["designatedCharityName"] == DBNull.Value ? "" : dt.Rows[i]["designatedCharityName"].ToString());
        //                listRoutes.isPrivate = (dt.Rows[i]["isPrivate"] == DBNull.Value ? false : (bool)dt.Rows[i]["isPrivate"]);
        //                listRoutes.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
        //                listRoutes.startingPoint = (dt.Rows[i]["startingPoint"] == DBNull.Value ? 0 : (int)dt.Rows[i]["startingPoint"]);
        //                listRoutes.latitude = (dt.Rows[i]["latitude"] == DBNull.Value ? "" : dt.Rows[i]["latitude"].ToString());
        //                listRoutes.longitude = (dt.Rows[i]["longitude"] == DBNull.Value ? "" : dt.Rows[i]["longitude"].ToString());
        //                listRoutes.country = (dt.Rows[i]["country"] == DBNull.Value ? "" : dt.Rows[i]["country"].ToString());
        //                listRoutes.state = (dt.Rows[i]["state"] == DBNull.Value ? "" : dt.Rows[i]["state"].ToString());
        //                listRoutes.cityName = (dt.Rows[i]["cityName"] == DBNull.Value ? "" : dt.Rows[i]["cityName"].ToString());
        //                listRoutes.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
        //                listRoutes.totalMiles = (dt.Rows[i]["totalMiles"] == DBNull.Value ? "" : dt.Rows[i]["totalMiles"].ToString());
        //                listRoutes.path = (dt.Rows[i]["path"] == DBNull.Value ? "" : dt.Rows[i]["path"].ToString());
        //                listRoutesDetails.Add(listRoutes);
        //            }
        //            return StatusCode((int)HttpStatusCode.OK, listRoutesDetails);
        //        }
        //        else
        //        {
        //            return StatusCode((int)HttpStatusCode.OK, listRoutesDetails);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        string SaveErrorLog = Data.Common.SaveErrorLog("selectRouteByUserId", e.Message);
        //        return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
        //    }
        //}
        //#endregion
        #region selectRouteById
        [HttpGet, Route("selectRouteById")]
        public IActionResult selectRouteById(int routeId)
        {
            dynamic listRoutes = new System.Dynamic.ExpandoObject();

            try
            {
                DataSet ds = Data.Route.selectRouteById(routeId);
                if (ds.Tables[0].Rows.Count > 0)
                {                       
                        listRoutes.routeId = (int)ds.Tables[0].Rows[0]["routeId"];
                        listRoutes.routeName = (ds.Tables[0].Rows[0]["routeName"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["routeName"].ToString());
                        listRoutes.comments = (ds.Tables[0].Rows[0]["comments"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["comments"].ToString());
                        listRoutes.designatedCharityId = (ds.Tables[0].Rows[0]["designatedCharityId"] == DBNull.Value ? 0 : (int)ds.Tables[0].Rows[0]["designatedCharityId"]);
                        listRoutes.designatedCharityName = (ds.Tables[0].Rows[0]["designatedCharityName"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["designatedCharityName"].ToString());
                        listRoutes.isPrivate = (ds.Tables[0].Rows[0]["isPrivate"] == DBNull.Value ? false : (bool)ds.Tables[0].Rows[0]["isPrivate"]);
                        listRoutes.createdDate = (ds.Tables[0].Rows[0]["createdDate"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["createdDate"].ToString());
                        listRoutes.startingPoint = (ds.Tables[0].Rows[0]["startingPoint"] == DBNull.Value ? 0 : (int)ds.Tables[0].Rows[0]["startingPoint"]);
                        listRoutes.filePath = (ds.Tables[0].Rows[0]["filePath"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["filePath"].ToString());
                        listRoutes.displayName = (ds.Tables[0].Rows[0]["displayName"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["displayName"].ToString());
                        listRoutes.role = (ds.Tables[0].Rows[0]["role"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["role"].ToString());
                        listRoutes.latitude = (ds.Tables[0].Rows[0]["latitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["latitude"].ToString());
                        listRoutes.longitude = (ds.Tables[0].Rows[0]["longitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["longitude"].ToString());
                        listRoutes.country = (ds.Tables[0].Rows[0]["country"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["country"].ToString());
                        listRoutes.state = (ds.Tables[0].Rows[0]["state"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["state"].ToString());
                        listRoutes.cityName = (ds.Tables[0].Rows[0]["cityName"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["cityName"].ToString());
                        listRoutes.address = (ds.Tables[0].Rows[0]["address"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["address"].ToString());
                        
                        List<dynamic> listRidePoints = new List<dynamic>();
                        dynamic ridePoints = new System.Dynamic.ExpandoObject();
                        for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                        {
                            ridePoints.routePointId  = (ds.Tables[1].Rows[j]["routePointId"] == DBNull.Value ? 0 : (int)ds.Tables[1].Rows[j]["routePointId"]);
                            ridePoints.displayId = (ds.Tables[1].Rows[j]["displayId"] == DBNull.Value ? 0 : (int)ds.Tables[1].Rows[j]["displayId"]);
                            ridePoints.filePath = (ds.Tables[1].Rows[j]["filePath"] == DBNull.Value ? "" : ds.Tables[1].Rows[j]["filePath"].ToString());
                            ridePoints.displayName = (ds.Tables[1].Rows[j]["displayName"] == DBNull.Value ? "" : ds.Tables[1].Rows[j]["displayName"].ToString());
                            ridePoints.role = (ds.Tables[1].Rows[j]["role"] == DBNull.Value ? "" : ds.Tables[1].Rows[j]["role"].ToString());
                            ridePoints.latitude  = (ds.Tables[1].Rows[j]["latitude"] == DBNull.Value ? "" : ds.Tables[1].Rows[j]["latitude"].ToString());
                            ridePoints.longitude = (ds.Tables[1].Rows[j]["longitude"] == DBNull.Value ? "" : ds.Tables[1].Rows[j]["longitude"].ToString());
                            ridePoints.country = (ds.Tables[1].Rows[j]["country"] == DBNull.Value ? "" : ds.Tables[1].Rows[j]["country"].ToString());
                            ridePoints.state = (ds.Tables[1].Rows[j]["state"] == DBNull.Value ? "" : ds.Tables[1].Rows[j]["state"].ToString());
                            ridePoints.cityName = (ds.Tables[1].Rows[j]["cityName"] == DBNull.Value ? "" : ds.Tables[1].Rows[j]["cityName"].ToString());
                            ridePoints.address = (ds.Tables[1].Rows[j]["address"] == DBNull.Value ? "" : ds.Tables[1].Rows[j]["address"].ToString());
                            listRidePoints.Add(ridePoints);
                        }
                        listRoutes.ridePoints = listRidePoints;
                        listRoutes.totalMiles = (ds.Tables[0].Rows[0]["totalMiles"] == DBNull.Value ? "" : String.Concat(ds.Tables[0].Rows[0]["totalMiles"], "mi").ToString());
                        listRoutes.path = (ds.Tables[0].Rows[0]["path"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["path"].ToString());
                    return StatusCode((int)HttpStatusCode.OK, listRoutes);

                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, "No Records Found");
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("selectRouteById", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion
        #region nearBySearch
        [HttpPost, Route("nearBySearch")]
        public async Task<IActionResult> nearBySearch(nearBySearch nearBySearch)
        { 
            try
            {
                Regex regex = new Regex(@"^\-?\d+\.?\d*$");
                System.Text.RegularExpressions.Match latitude = regex.Match(nearBySearch.latitude);
                System.Text.RegularExpressions.Match longitude = regex.Match(nearBySearch.longitude);
                if (latitude.Success & longitude.Success)
                {
                    using (var client = new HttpClient())
                    {
                        var a = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + latitude + "," + longitude + "&radius=5000&keyword="+nearBySearch.category+"&key=" + Common.Apikey();
                        var response = await client.GetStringAsync(string.Format(a));
                        List<string> result = new List<string>();
                        result.Add(response);
                        O: 
                        JObject o = JObject.Parse(@response);
                        List<JToken> isNextPageToken = o.SelectTokens("$.['next_page_token']").ToList();
                        if(isNextPageToken.Count > 0)  //next - page - token
                        {
                            var b = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?pagetoken=" + isNextPageToken[0].ToString() + "&key=" + Common.Apikey();
                            response = await client.GetStringAsync(string.Format(b));
                            result.Add(response);
                            goto O;
                        }
                        return StatusCode((int)HttpStatusCode.OK, result);
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Invalid latitude/longitude" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = SpiritMeter.Data.Common.SaveErrorLog("nearBySearch", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });

            }
        }
        #endregion createUser
    }
}