﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritMeter.Models
{
    
        public class createDisplay
        {
            public int displayId { get; set; }
            public string name { get; set; }
            public string notes { get; set; }
            public string markerType { get; set; }
            [DefaultValue(false)]
            public bool? isPrivate { get; set; }
            public int createdBy { get; set; }
            public string markerUrl { get; set; }
    }

    public class updateDisplay: createDisplay
    {
        
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string cityName { get; set; }
        public string address { get; set; }
        public string filePath { get; set; }
    }

    public class createDisplayFiles
        {
           
            public int displayId { get; set; }
            public string filePath { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string country { get; set; }
            public string state { get; set; }
            public string cityName { get; set; }
            public string address { get; set; }
        }
        public class Global
        {
            public  string fileurl { get; set; }
        }

    public class CreatelDisplayCharity
    {
        public int displayId { get; set; }
        public int charityId { get; set; }
    }
    public class UpdateDisplayCharity: CreatelDisplayCharity
    {
        public int displayCharityId { get; set; }
    }
}
