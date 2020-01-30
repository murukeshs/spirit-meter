using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritMeter.Models
{
   public class CharityModel
   {

    public class CreateCharity
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string password { get; set; }
        public string gender { get; set; }
        public string profileImage { get; set; }
        public string email { get; set; }
        public string address { get; set; }
    }

        public class UpdateCharity
        {
            [Required]
            public int charityId { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string phoneNumber { get; set; }
            public string gender { get; set; }
            public string profileImage { get; set; }
            public string email { get; set; }
            public string address { get; set; }
        }

    }
}
