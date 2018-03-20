using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NetCalendar.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            string strCulture = (Request.Cookies["NetCalendar"] != null) ? Request.Cookies["NetCalendar"].Value : "uk-UA";

            if (strCulture != string.Empty)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(strCulture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(strCulture);
            }
        }
    }
}
