        using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HolidayApp.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiritMeter.Data;

namespace HolidayApp.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class EventController : ControllerBase
    {
        #region searchEvents
        /// <summary>
        /// To search Events 
        /// </summary>
        [HttpPost, Route("searchEvents")]
        public async Task<IActionResult> searchEvents(eventPoints eventPoints)
        {
            try
            {
                Regex regex = new Regex(@"^\-?\d+\.?\d*$");
                System.Text.RegularExpressions.Match latitude = regex.Match(eventPoints.latitude);
                System.Text.RegularExpressions.Match longitude = regex.Match(eventPoints.longitude);
                if (latitude.Success & longitude.Success)
                {
                    using (var client = new HttpClient())
                    {
                        string url = "https://www.eventbriteapi.com/v3/events/search/?location.within=5km&location.longitude=" + longitude + "&location.latitude=" + eventPoints.latitude + "&categories="+ eventPoints.categories.Replace(",", "%2C") + "&date_modified.range_end=" + eventPoints.from.Replace(":", "%3A") +  "&token="+Common.EventToken();
                        //string url = "https://www.eventbriteapi.com/v3/events/search/?location.within=5km&location.longitude=" + longitude + "&location.latitude=" + eventPoints.latitude + "&categories=" + category + "&start_date.range_start=" + from + "&start_date.range_end=" + eventPoints.to.Replace(":", "%3A") + "&token=" + Common.EventToken();
                        var response = await client.GetStringAsync(url);

                        return Ok(response);

                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Invalid latitude/longitude" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = SpiritMeter.Data.Common.SaveErrorLog("searchEvents", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
               
            }
        }
        #endregion createUser


    }
}