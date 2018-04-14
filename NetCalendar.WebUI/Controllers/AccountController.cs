using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NetCalendar.WebUI.IdentityInfrastructure;
using NetCalendar.WebUI.Models.Identity;
using NetCalendar.Domain;
using NetCalendar.WebUI.Models;

namespace NetCalendar.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly NetCalendarService netCalendarService;

        public AccountController(NetCalendarService service)
        {
            netCalendarService = service;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            ViewData["Dep"] = DepartmentList();
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel details, string returnUrl)
        {
            string userName;

            if (details.Name == null)
                userName = details.Department;
            else
                userName = details.Name + "#"+details.Department;

            AppUser user = await UserManager.FindAsync(userName, details.Password);
            if (user == null)
            {
                return View("Error", new string[] { "Некорректное имя или пароль." });
            }
            else
            {
                ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                AuthManager.SignOut();
                AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
				
                if (returnUrl == "")
                    return RedirectToAction("Index", "Home");
                else
                    return Redirect(returnUrl);
            }
        }


        [Authorize]
        public ActionResult Logout()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");
        }



        public ActionResult Register(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;

            ViewData["Dep"] = DepartmentList();
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Register(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { UserName = model.Name + "#" + model.Department, Email = model.Email, Department = model.Department, IsManager = model.IsManager };

                if (user.IsManager)
                {
                    AppRole role = RoleManager.FindByName("Manager");

                    if (role == null)
                        return View("Error", new string[] { "Not Role" });
                    try
                    {
                        await UserManager.CreateAsync(user, model.Password);
                        UserManager.AddToRole(user.Id, role.Name);
                        return RedirectToAction("Index", "Admin");
                    }
                    catch (Exception ex)
                    {
                        return View("Error", new string[] {"reg "+ ex.Message });
                    }
                }
                else
                {
                    AppUser existUser = UserManager.FindByName(user.UserName);

                    if (existUser != null && existUser.PasswordHash != null)
                        return RedirectToAction("Index", "Home");

                    else
                    {
                        existUser.Email = model.Email;

                        try
                        {
                            IdentityResult res = UserManager.Update(existUser);

                            if (!res.Succeeded)
                            {
                                return View("Error", new string[] { "Not register employee" });
                            }

                            UserManager.AddPassword(existUser.Id, model.Password);
                            await UpdateEventsAsync(existUser);

                            return RedirectToAction("Index", "Home");
                        }
                        catch (Exception ex)
                        {
                            return View("Error", new string[] { "reg " + ex.Message });
                        }
                    }
                    
                }
            }
            return View("Error", new string[] { "Invalid model" });
        }

        private async Task UpdateEventsAsync(AppUser existUser)
        {
            List<Meeting> userEvents = netCalendarService.GetMeetingsOfEmployee(existUser.ToEmployee(), DateTime.Now.AddDays(0), DateTime.Now.AddDays(365));
            if(userEvents.Count!=0)
            {
                foreach (Meeting ev in userEvents)
                    await netCalendarService.SaveUpdateEventAsync(ev);
            }
        }

        public async Task<bool> CreateRole(string name)
        {
           IdentityResult result = await RoleManager.CreateAsync(new AppRole(name));
           return result.Succeeded;
        }


        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }


        private AppRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
            }
        }


        public JsonResult GetUsers(string department)
        {
            List<AppUser> empl = UserManager.GetUsersByDepartment(department);
            return new JsonResult { Data = empl, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IEnumerable<SelectListItem> DepartmentList()
        {
            IEnumerable<SelectListItem> selectListItems = new List<SelectListItem>();

            selectListItems = from deprtmentName in UserManager.GetDepartments()
                              select new SelectListItem
                              {
                                  Text = deprtmentName,
                                  Value = deprtmentName
                              };


            return selectListItems;

        }
    }
}