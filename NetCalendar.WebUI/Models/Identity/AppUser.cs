using Microsoft.AspNet.Identity.EntityFramework;
using NetCalendar.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetCalendar.WebUI.Models.Identity
{
    public class AppUser : IdentityUser
    {
        public string Department{get; set;}
        public bool IsManager { get; set; }
        public string Adress { get; set; }
        public string SimplyName
        {
            get { return UserName.Split('#')[0]; }
            private set { }
        }

        public Employee ToEmployee()
        {
            Employee employee = new Employee()
            {
                Department =this.Department,
                Email = this.Email,
                IsManager = this.IsManager,
                Name = this.SimplyName
            };
            return employee;
        }
    }
}