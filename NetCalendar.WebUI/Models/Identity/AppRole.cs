using Microsoft.AspNet.Identity.EntityFramework;

namespace NetCalendar.WebUI.Models.Identity
{
    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }

        public AppRole(string name) : base(name) { }
    }
}