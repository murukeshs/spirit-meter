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

    public class displayList
    {
        public int displayId { get; set; }
        public string name { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string notes { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string cityName { get; set; }
        public string address { get; set; }
        public string type { get; set; }
        public bool isPrivate { get; set; }
        public string createdDate { get; set; }
        public int createdBy { get; set; }
        public string createdByName { get; set; }
        public string filePath { get; set; }
        public string routes { get; set; }
    }
    public class displayLists
    {
        public int displayId { get; set; }
        public string name { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string notes { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string cityName { get; set; }
        public string address { get; set; }
        public string type { get; set; }
        public bool isPrivate { get; set; }
        public string createdDate { get; set; }
        public int createdBy { get; set; }
        public string createdByName { get; set; }
        public string filePath { get; set; }
        public string routes { get; set; }
    }
    public class routeList
    {
        public int routeId { get; set; }
        public string routeName { get; set; }
        public string comments { get; set; }
        public int designatedCharityId { get; set; }
        public string designatedCharityName { get; set; }
        public bool isPrivate { get; set; }
        public string createdDate { get; set; }
        public int startingPoint { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string cityName { get; set; }
        public string address { get; set; }
        public string totalMiles { get; set; }
        public string path { get; set; }
        public string routes { get; set; }
    }
    public class RefreshRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public DateTimeOffset AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }
    }
}
