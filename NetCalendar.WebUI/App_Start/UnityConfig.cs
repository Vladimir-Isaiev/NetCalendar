

using Microsoft.AspNet.Identity.EntityFramework;
using NetCalendar.DataLayer;
using NetCalendar.Domain;
using NetCalendar.Repo;
using NetCalendar.WebUI.IdentityInfrastructure;
using NetCalendar.WebUI.Models.Identity;
using System;
using System.Web;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace NetCalendar.WebUI
{    
    public static class UnityConfig
    {
        private static Lazy<IUnityContainer> container =
                                                          new Lazy<IUnityContainer>(() =>
                                                          {
                                                              var container = new UnityContainer();
                                                              RegisterTypes(container);
                                                              return container;
                                                          });

        
        public static IUnityContainer Container => container.Value;
        




        public static void RegisterTypes(IUnityContainer container)
        {
            String path_dir = HttpRuntime.AppDomainAppPath;

            InjectionConstructor googleServiceConstructor =
                new InjectionConstructor(new GoogleService(path_dir, new NotificationService()));

            container.RegisterType<IServiceMeeting, GoogleRepo>(googleServiceConstructor);

            container.RegisterType<INotificationService, NotificationService>();

        }
    }
}