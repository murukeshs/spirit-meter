using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritMeter.Models
{

    public class createRoute
    {
        public int routeId { get; set; }
        public string routeName { get; set; }
        public string comments { get; set; }
        public int? designatedCharityId { get; set; }
        public int? startingPoint { get; set; }
        [DefaultValue(false)]
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
        public string status { get; set; }
    }
    public class calculatepath
    {
        public string displayId { get; set; }
        public int startingPoint { get; set; }
    }
    public class nearBySearch
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string category { get; set; }
    }
}