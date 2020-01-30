using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritMeter.Models
{
    public class AdsModule
    {
        public class CreateAd
        {
            public string name { get; set; }
            public string description { get; set; }
            public string navigationURL { get; set; }
            public string image { get; set; }
            public string priority { get; set; }
            public string expiryDate { get; set; }
            public bool adStatus { get; set; }
        }

        public class UpdateAd: CreateAd
        {
            [Required]
            public int adId { get; set; }
            
        }
    }
}
