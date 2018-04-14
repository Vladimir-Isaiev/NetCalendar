using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NetCalendar.Domain;
using NetCalendar.Repo;
using NetCalendar.WebUI.IdentityInfrastructure;
using NetCalendar.WebUI.Models;
using NetCalendar.WebUI.Models.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace NetCalendar.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly NetCalendarService netCalendarService;

        public HomeController(NetCalendarService service)
        {
            netCalendarService = service;
        }

        
        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        public ActionResult Calendar()
        {
            ViewBag.Department = CurrentUser.Department;
            ViewBag.IsManager = CurrentUser.IsManager.ToString();
            return View();
        }


       

       

        public JsonResult GetMeetings()
        {
            List<Meeting> meetings;
            if (CurrentUser.IsManager)
                meetings = netCalendarService.GetMeetingsOfDepartment(CurrentUser.Department, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(365));
            else
                meetings = netCalendarService.GetMeetingsOfEmployee(CurrentUser.ToEmployee(), DateTime.Now.AddDays(-30), DateTime.Now.AddDays(365));

           
            return new JsonResult { Data = meetings, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

       


        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> SaveMeetingAsync(Meeting newMeeting)
        {
            bool status = false;
            List<string> employeesNames;
            string message = "";


            List<Employee> employees = new List<Employee>();
            List<AppUser> users = UserManager.GetUsersByDepartment(newMeeting.Department);

            if (newMeeting.Description != null)
            {
                char[] toTrim = { ' ', ',' };
                employeesNames = newMeeting.Description.Trim(toTrim).Split(',').ToList();


                employeesNames.ForEach(delegate (string name)
                {
                    name = name.Trim() + "#" + newMeeting.Department;
                    employees.Add(UserManager.FindByName(name).ToEmployee());
                });

                newMeeting.Employees = employees;
            }


            try
            {
                if (ModelState.IsValid)
                {
                    await netCalendarService.SaveUpdateEventAsync(newMeeting);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                message = "Error: " + ex.Message;
                while (ex.InnerException != null)
                {
                    string temp = ex.Message;
                    ex = ex.InnerException;
                    if (!temp.Equals(ex.Message))
                        message += "--->" + ex.Message;
                }
            }
            
            return new JsonResult { Data = new { status = status, mess = message } };
        }


        [HttpPost]
        public JsonResult DeleteMeeting(string googleId, string department)
        {
            bool status = false;
            string message = "";
            try
            {
                netCalendarService.DeleteMeeting(googleId, department);
                status = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                while (ex.InnerException != null)
                {
                    string temp = ex.Message;
                    ex = ex.InnerException;
                    if (!temp.Equals(ex.Message))
                        message += "--->" + ex.Message;
                }
            }
            

            return new JsonResult { Data = new { status = status, mes = message } };
        }

        
        public JsonResult GetUsers(string department)
        {
            List<AppUser> empl = UserManager.GetUsersByDepartment(department);
            return new JsonResult { Data = empl, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult GetMeetingsDuration(string start, string end)//"09/03/2018 0:00"   DD/MM/YYYY
        {            
            CultureInfo provider = new CultureInfo("en-UK");
            DateTime dStart = DateTime.ParseExact(start, "dd/MM/yyyy HH:mm", provider);
            DateTime dEnd = DateTime.ParseExact(end, "dd/MM/yyyy HH:mm", provider);
            
            int summ;
            string _summa;
            if (CurrentUser.IsManager)
            {
                summ = netCalendarService.GetMeetingsDuration(CurrentUser.Department, dStart, dEnd);
                _summa = "Number of hours worked by the department " + CurrentUser.Department + ": "+summ.ToString();
            }
            else
            {
                summ = netCalendarService.GetMeetingsDuration(CurrentUser.ToEmployee(), dStart, dEnd);
                _summa = "Number of hours worked by " + CurrentUser.SimplyName + ": " + summ.ToString();
            }

            return new JsonResult { Data = new { summa = _summa } };
        }


        public PartialViewResult SetCulture()
        {
            CultureModel model = new CultureModel();
            model.SelectedKeyCulture = Request.Cookies["NetCalendar"] != null ? Request.Cookies["NetCalendar"].Value : "uk-UA";
            return PartialView("_SetCulture", model);
        }


        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        private AppUser CurrentUser
        {
            get
            {
                string userName = HttpContext.User.Identity.Name;
                List<AppUser> users = UserManager.Users.ToList();
                AppUser user = users.Find(u => u.UserName.Equals(userName));
                return user;
            }
        }
    }
}