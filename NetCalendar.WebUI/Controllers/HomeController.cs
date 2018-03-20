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
        private readonly NetCalendarService repository;

        public HomeController(NetCalendarService repo)
        {
            repository = repo;
        }

        
        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        public ActionResult Calendar()
        {
            ViewBag.Department = _User.Department;
            ViewBag.IsManager = _User.IsManager.ToString();
            return View();
        }


       

       

        public JsonResult GetEvents()
        {
            List<Event> events;
            if (_User.IsManager)
                events = repository.GetEventsOfDepartment(_User.Department, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(365));
            else
                events = repository.GetEventsOfEmployee(_User.ToEmployee(), DateTime.Now.AddDays(-30), DateTime.Now.AddDays(365));

           
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

       


        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> SaveEventAsync(Event e)
        {
            bool status = false;
            string[] names;
            string message = "Please wait";


            List<Employee> employees = new List<Employee>();
            List<AppUser> users = UserManager.GetUsersByDepartment(e.Department);

            if (e.Description != null)
            {
                names = e.Description.Trim().Trim(',').Split(',').ToArray();



                for (int i = 0; i < names.Count(); ++i)
                {
                    names[i] = names[i].Trim() + "#" + e.Department;
                    employees.Add(UserManager.FindByName(names[i]).ToEmployee());
                }
                e.Employees = employees;
            }


            try
            {
                if (ModelState.IsValid)
                {
                    await repository.SaveUpdateEventAsync(e);
                    status = true;
                }
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
            
            return new JsonResult { Data = new { status = status, mess = message } };
        }


        [HttpPost]
        public JsonResult DeleteEvent(string googleId, string department)
        {
            bool status = false;
            string message = "";
            try
            {
                repository.DeleteEvent(googleId, department);
                status = true;
            }catch(Exception ex)
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
        public JsonResult SumEvent(string start, string end)//"09/03/2018 0:00"
        {
            DateTime dStart = new DateTime(
                Int32.Parse(start.Substring(6, 4)),
                Int32.Parse(start.Substring(3, 2)),
                Int32.Parse(start.Substring(0, 2))
                );

            DateTime dEnd = new DateTime(
               Int32.Parse(end.Substring(6, 4)),
               Int32.Parse(end.Substring(3, 2)),
               Int32.Parse(end.Substring(0, 2))
               );
            int summ;
            string sum;
            if (_User.IsManager)
            {
                summ = repository.SumEventDepartment(_User.Department, dStart, dEnd);
                sum = "Number of hours worked by the department " + _User.Department + ": "+summ.ToString();
            }
            else
            {
                summ = repository.SumEventUser(_User.ToEmployee(), dStart, dEnd);
                sum = "Number of hours worked by " + _User.SimplyName + ": " + summ.ToString();
            }

            return new JsonResult { Data = new { summa = sum } };
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

        private AppUser _User
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