using System;
using System.Collections.Generic;

namespace PointOfBeauty.Data
{
    public static class UserStore
    {
        public static List<User> Users { get; set; } = new List<User>();

        public class User
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        }

        public class Appointment
        {
            public string ServiceType { get; set; }
            public DateTime DateTime { get; set; }
        }

        public static User GetUserByEmail(string email)
        {
            return Users.Find(u => u.Email == email);
        }
    }
}