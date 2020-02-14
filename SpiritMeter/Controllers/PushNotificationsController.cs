using System;
using System.Collections.Generic;
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




namespace HolidayApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PushNotificationsController : ControllerBase
    {

        [HttpPost, Route("sendmessages")]
        public IActionResult SendMessage(/*PushNotification pushNotification*/)
        {
            try
            {
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
                    to = "eQZuo4EdcbA:APA91bFbtAdWJjwTduzDdTq1PoJwB0BN7LSm8IbrMUMsLsViZmIIrx5gHD8HBFGBFttZVa9MWSyb8QkRnlY4IfGPD7IziWU2yReNb9r6SMtQQELvwV5NY4P1xjnMFZyhWz16LBNDmapr", //deviceId,
                    priority = "high",
                    content_available = true,
                    notification = new
                    {
                        body = "you are near by PG in 6.76 km distance", //txtmsg,
                        title = "Near By Display",//txttitle.Replace(":", ""),
                        sound = "default"
                    },
                };

                //var payload = new
                //{
                //    pushNotification.to, //deviceId,
                //    priority = "high",
                //    content_available = true,
                //    //notification = new
                //    //{
                //    //    body = pushNotification.Body, //txtmsg,
                //    //    title = pushNotification.Title,//txttitle.Replace(":", ""),
                //    //    sound = "default"
                //    //},
                //    pushNotification.notification,
                //    pushNotification.data,
                //}; 


                var json =JsonSerializer.Serialize(payload);
               

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
                return StatusCode((int)HttpStatusCode.OK, sResponseFromServer);

                //return StatusCode((int)HttpStatusCode.OK, "Sent Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ex);
            }
        }
    }
}