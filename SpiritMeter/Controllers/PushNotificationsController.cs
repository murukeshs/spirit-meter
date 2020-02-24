using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HolidayApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace SpiritMeter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PushNotificationsController : ControllerBase
    {

        [HttpPost, Route("sendmessages")]
        public IActionResult SendMessage(RouteNotification routeNotification)
        {

           // List<dynamic> listUserDevice = new List<dynamic>();
            dynamic notification = new System.Dynamic.ExpandoObject();
            dynamic user = new System.Dynamic.ExpandoObject();
            try
            {

                DataSet ds = Data.User.GetRouteNotification(routeNotification);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                       
                        user.firebaseRegID = (ds.Tables[1].Rows[0]["firebaseRegID"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["firebaseRegID"].ToString());

                        //listUserDevice.Add(user);
                    }

                    //displayInfo
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dynamic listDisplay = new System.Dynamic.ExpandoObject();

                        listDisplay.displayId        = (int)ds.Tables[0].Rows[i]["displayId"];
                        listDisplay.name            = (ds.Tables[0].Rows[i]["name"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["name"].ToString());
                        listDisplay.latitude        = (ds.Tables[0].Rows[i]["latitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["latitude"].ToString());
                        listDisplay.longitude       = (ds.Tables[0].Rows[i]["longitude"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["longitude"].ToString());
                        listDisplay.country         = (ds.Tables[0].Rows[i]["country"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["country"].ToString());
                        listDisplay.state           = (ds.Tables[0].Rows[i]["state"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["state"].ToString());
                        listDisplay.cityName        = (ds.Tables[0].Rows[i]["cityName"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["cityName"].ToString());
                        listDisplay.address         = (ds.Tables[0].Rows[i]["address"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["address"].ToString());
                        listDisplay.markerType      = (ds.Tables[0].Rows[i]["markerType"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["markerType"].ToString());
                        listDisplay.markerUrl       = (ds.Tables[0].Rows[i]["markerUrl"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["markerUrl"].ToString());
                        listDisplay.distance        = (ds.Tables[0].Rows[i]["distance"] == DBNull.Value ? "" : ds.Tables[0].Rows[i]["distance"].ToString());
                        listDisplay.navigationUrl   = "http://www.google.com/maps/place/" + listDisplay.latitude+ ","+listDisplay.longitude;
                        listDisplay.body = "you are near by " + listDisplay.name + " in " + listDisplay.distance + " km distance";
                        listDisplay.title = "Near By Lighting Display";

                        //notificationInfo
                        //notification.displayId = listDisplay.displayId;
                        //notification.body = "you are near by " + listDisplay.name + " in " + listDisplay.distance + " km distance";
                        //notification.title = "Near By Lighting Display";
                        
                        //notification.icon = listDisplay.markerUrl;

                       // List<string> listUserDevice = new List<string>() { "eb__pHGsayI:APA91bGJw8xY5dyQ2BZTrHJ4BkVAR6LfkXS1CN-Vr0IT08zXER08qVAcohY8zfgKHXhxNcTgJMlsGOcYqVf0Ac9q0fQHhE1RhRCnafL3FtlYA9RVIl5OMuac_rr2W--phkE-m-h-WU8Q" };
                        //listUserDevice.Add("fOym5P7ommo:APA91bFVKHxf3JSCy4e70by_G2eyhRQQ-wERrTcyN-pI0gFrBETxjwMIyvzjc4k9cjfnjfg_AsZswPqi-a_KLz0WuTWr3UrQ6I3NUcNKEI10K9GfHOnreu3AAHLxF4mwOPdZ4u4r8tC2");
                        //listUserDevice.Add("eb__pHGsayI:APA91bGJw8xY5dyQ2BZTrHJ4BkVAR6LfkXS1CN-Vr0IT08zXER08qVAcohY8zfgKHXhxNcTgJMlsGOcYqVf0Ac9q0fQHhE1RhRCnafL3FtlYA9RVIl5OMuac_rr2W--phkE-m-h-WU8Q");

                        //string DeviceId = "fOym5P7ommo:APA91bFVKHxf3JSCy4e70by_G2eyhRQQ-wERrTcyN-pI0gFrBETxjwMIyvzjc4k9cjfnjfg_AsZswPqi-a_KLz0WuTWr3UrQ6I3NUcNKEI10K9GfHOnreu3AAHLxF4mwOPdZ4u4r8tC2";
                        string SERVER_API_KEY = "AAAAG4H6RTM:APA91bESx_AuSDH-4y9lkpp4Ujwd38IjqfFXWXVxV0HWO8YO49WVrZXnWqMNufUtI5CeVXIJVNX2PWSMWb0u9mRjCrZSIvH8P7KNvud7hGjTOg6tpLXfjwFyCNv5swFWjzquVRrrSAF0";
                        var SENDER_ID = "118144779571";

                        //Create the web request with fire base API  
                        WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                        tRequest.Method = "post";
                        //serverKey - Key from Firebase cloud messaging server  
                        tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
                        //Sender Id - From firebase project setting  
                        tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
                        tRequest.ContentType = "application/json";


                        var payload = new
                        {
                            to = user.firebaseRegID, //deviceId,
                            priority = "high",
                            content_available = true,
                            //notification = new
                            //{

                            //    body = notification.body, //txtmsg,
                            //    title = notification.title,//txttitle.Replace(":", ""),
                            //    icon = listDisplay.markerUrl,
                            //    sound = "default",
                            //    click_action= "com.android.holidaydrive_TARGETNOTIFICATION",
                            //    listDisplay.displayId,
                            //    listDisplay.name,
                            //    listDisplay.latitude,
                            //    listDisplay.longitude,
                            //    listDisplay.country,
                            //    listDisplay.state,
                            //    listDisplay.cityName,
                            //    listDisplay.address,
                            //    listDisplay.markerType,
                            //    listDisplay.distance,
                            //    listDisplay.navigationUrl
                            //},
                           
                            data = listDisplay
                        };

                       



                        var json = JsonSerializer.Serialize(payload);

                        Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                        tRequest.ContentLength = byteArray.Length;

                        Stream dataStream = tRequest.GetRequestStream();
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();

                        WebResponse tResponse = tRequest.GetResponse();
                        dataStream = tResponse.GetResponseStream();
                        StreamReader tReader = new StreamReader(dataStream);
                        String sResponseFromServer = tReader.ReadToEnd();

                        tReader.Close();
                        dataStream.Close();
                        tResponse.Close();
                        JObject o = JObject.Parse(@sResponseFromServer);
                        JToken msg = o.SelectToken("$.['success']");
                        if(Convert.ToInt32(msg) == 1)
                        {
                            DataSet ds1 = Data.User.SaveUserNotification(routeNotification.userId, routeNotification.routeId, notification.displayId);

                        }
                        else
                        {
                            
                            return StatusCode((int)HttpStatusCode.InternalServerError, sResponseFromServer);
                        }

                    }
                   
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "No display is nearby 10 km from your Location");
                }
               
                return StatusCode((int)HttpStatusCode.OK, "Notification Send Successfully");

                //return StatusCode((int)HttpStatusCode.OK, "Sent Successfully");
            }
            catch (Exception ex)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("Service: Call push notification", ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ex);
            }
        }
    }
}