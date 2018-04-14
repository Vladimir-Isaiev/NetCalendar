using NetCalendar.WebUI.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetCalendar.WebUI.IdentityInfrastructure
{
    public static class AppUserManagerExtension
    {


        public static List<AppUser> GetUsersByDepartment(this AppUserManager appUserManager, string dep)
        {
            List<AppUser> users = new List<AppUser>();
            //users = appUserManager.Users.ToList();
            users = appUserManager.Users.ToList().FindAll(u => u.Department.Equals(dep));

            return users;
        }


        public static List<string> GetDepartments(this AppUserManager appUserManager)
        {
            List<string> departments = new List<string>();
            List<AppUser> users = appUserManager.Users.ToList();

            foreach (AppUser user in users)
            {
                if (!departments.Contains(user.Department))
                    departments.Add(user.Department);
            }

            return departments;
        }
    }
}