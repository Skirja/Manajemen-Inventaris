using System;

namespace Manajemen_Inventaris.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        public User()
        {
        }

        public User(string username, string password, string email, string company)
        {
            Username = username;
            Password = password;
            Email = email;
            Company = company;
        }
    }
}