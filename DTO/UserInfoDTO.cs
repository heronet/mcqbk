using System;
using System.Collections.Generic;

namespace DTO
{
    public class UserInfoDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<string> Roles { get; set; }
    }
}