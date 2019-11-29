using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritMeter.Models
{

    public class createRoute
    {
        public int routeId { get; set; }
        public string routeName { get; set; }
        public string comments { get; set; }
        public int? userId { get; set; }
        public bool? isPrivate { get; set; }
    }
   

    public class routePoints
    {
        public int routePointId { get; set; }
        public int routeId { get; set; }
        public string displayId { get; set; }
        public int startingPoint { get; set; }
    }
    public class routePointStatus
    {
        public int routePointId { get; set; }
        public int userId { get; set; }
    }

    public class rideStatus
    {
        public int rideStatusId  { get; set; }
        public int routeId { get; set; }
        public int userId { get; set; }
        public DateTime rideStartTime { get; set; }
        public DateTime rideCompleteTime { get; set; }
        public string status { get; set; }
    }
   
}