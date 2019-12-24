﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizedServer.Models
{
    public class UserInfo
    {

        public string UserName { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public static IList<UserInfo> GetAllUsers()
        {
            return new List<UserInfo>()
            {
                new UserInfo {  ClientId="100",ClientSecret="888", UserName="Member",Password="123" },
                new UserInfo {  ClientId="101",ClientSecret="999", UserName="Order",Password="123" },
            };
        }
    }
}