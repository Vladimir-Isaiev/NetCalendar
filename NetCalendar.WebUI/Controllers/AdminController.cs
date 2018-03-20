using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NetCalendar.WebUI.IdentityInfrastructure;
using NetCalendar.WebUI.Models.Identity;

namespace NetCalendar.WebUI.Controllers
{
    [Authorize]
    [Authorize(Roles = "Manager")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            AppUser user = _User;
            List<AppUser> users = UserManager.GetUsersByDepartment(user.Department);
            return View(users);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(AddEmplModel model)
        {
            if (ModelState.IsValid)
            {
                string department = _User.Department;

                AppUser user = new AppUser { UserName = model.Name+"#"+department, Department = department };

                IdentityResult result = await UserManager.CreateAsync(user);
                
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);

            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "Пользователь не найден" });
            }
        }


        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }




        [HttpPost]
        public async Task<ActionResult> Edit(string id, string SimplyName)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                user.UserName = SimplyName + "#"+_User.Department;
                
                IdentityResult result = await UserManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                
            }
            else
            {
                ModelState.AddModelError("", "Пользователь не найден");
            }

            return View(user);
        }



        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
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