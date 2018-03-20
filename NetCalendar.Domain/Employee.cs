using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NetCalendar.Domain
{
    public class Employee
    {
         public string Name { get; set; }

         public string Department { get; set; }

         public string Email { get; set; }

         public string Phone { get; set; }

        public string Adress { get; set; }

        public bool IsManager { get; set; }
    }
}