using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayApp.Models
{
    public class PushNotification
    {
        public List<string> to { get; set; }
        //public string Title { get; set; }
        //public string Body { get; set; }
        public List<string> notification { get; set; }
        public List<string> data { get; set; }
    }

    public class RouteNotification
    {
        public int userId { get; set; }
        public int routeId { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
}
