using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using NetCalendar.WebUI.Models.Identity;

namespace NetCalendar.WebUI.IdentityInfrastructure
{
    public class AppRoleManager : RoleManager<AppRole>, IDisposable
    {
        public AppRoleManager(RoleStore<AppRole> store) : base(store) { }


        public static AppRoleManager Create(IdentityFactoryOptions<AppRoleManager> options, IOwinContext context)
        {
            AppIdentityDbContext dc = context.Get<AppIdentityDbContext>();
            return new AppRoleManager(new RoleStore<AppRole>(dc));
        }
    }
}