using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetCalendar.WebUI.Models
{
    public class Departments
    {
        List<string> listDepartment;
        public Departments()
        {
            listDepartment = new List<string>();
        }


        public List<string> ListDepartment
        {
            get {  return listDepartment; }
            set { listDepartment = value; }
        }


        public bool AddDepartment(string department)
        {
            bool isOk = false;

            if (!ListDepartment.Contains(department))
            {
                ListDepartment.Add(department);
                isOk = true;
            }
            return isOk;
        }


        public bool RemoveDepartment(string department)
        {
            bool isOk = false;

            if (ListDepartment.Contains(department))
            {
                ListDepartment.Remove(department);
                isOk = true;
            }
            return isOk;
        }
    }
}