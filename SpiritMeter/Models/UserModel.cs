using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritMeter.Models
{
    public class Login
    {
        public string phoneNo { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }

    public class createUser
    {
        public int userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string profileImage { get; set; }
        public string gender { get; set; }
        public string role { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string cityName { get; set; }
        public string address { get; set; }
        public string password { get; set; }
    }

    public class GenerateOTP
    {
        public string phone { get; set; }
    }

    public class forgotPassword
    {
        public string phone { get; set; }
        public string password { get; set; }
        public string OTPValue { get; set; }
    }
   
}
