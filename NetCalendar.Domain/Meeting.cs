using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NetCalendar.Domain
{
    public class Meeting
    {
        public Meeting()
        {
            Employees = new List<Employee>();
        }

        public String Subject { get; set; }

        public string Department { get; set; }

         public string GoogleEventId { get; set; }

         public String Description { get; set; }

         public DateTime Start { get; set; }

          public DateTime End { get; set; }

          public String ThemeColor { get; set; }

        public String Adress { get; set; }

        public string DestLat { get; set; }
        public string DestLong { get; set; }

        public int Offset { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }

    
}