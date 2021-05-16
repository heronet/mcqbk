using System;

namespace DTO
{
    public class UserInfoDTO
    {
        public DateTime? LastActive { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}