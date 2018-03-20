using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NetCalendar.WebUI.Models.Identity;

namespace NetCalendar.WebUI.IdentityInfrastructure
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext() : base("name=IdentityDbAzure") { }

        static AppIdentityDbContext()
        {
            Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
        }
        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }


    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<AppIdentityDbContext>
    {
        protected override void Seed(AppIdentityDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }
        public void PerformInitialSetup(AppIdentityDbContext context)
        {
            AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));
            AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));

            string roleAdmin = "Admin";
            string roleManager = "Manager";

            string NameAdmin = "AdminNetCalendar";
            string password = "000000";
            string email = "netcalendarmanager@gmail.com";

            if (!roleMgr.RoleExists(roleAdmin))
            {
                roleMgr.Create(new AppRole(roleAdmin));
            }

            if (!roleMgr.RoleExists(roleManager))
            {
                roleMgr.Create(new AppRole(roleManager));
            }

            AppUser user = userMgr.FindByName(NameAdmin);
            if (user == null)
            {
                user = new AppUser();
                user.Adress = "";
                user.Department = "AdminNetCalendar";
                user.Email = email;
                user.IsManager = true;
                user.UserName = NameAdmin;
              }
            
            
                userMgr.Create(user, password);
                user = userMgr.FindByName(NameAdmin);
            
           if(user!=null)
            {
                if (!userMgr.IsInRole(user.Id, roleAdmin))
                {
                    userMgr.AddToRole(user.Id, roleAdmin);
                }

                if (!userMgr.IsInRole(user.Id, roleManager))
                {
                    userMgr.AddToRole(user.Id, roleManager);
                }
            }

           
        }
    }


}