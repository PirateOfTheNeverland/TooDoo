using TooDooSvc.Logging;
using TooDooSvc.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;

namespace TooDooWebRole
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        //private static IUnityContainer _container;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DependenciesConfig.RegisterDependencies();
            AuthConfig.RegisterAuth();

            PhotoService photoService = new PhotoService(new Logger());
            photoService.CreateAndConfigureAsync();

            //_container = new UnityContainer();
            //_container.RegisterType<IPhotoService, PhotoService>();
            //_container.RegisterType<TooDooWebRole.Controllers.TasksController>();
            //_container.RegisterType<TooDooWebRole.Controllers.DashboardController>();
            //_container.RegisterType<TooDooWebRole.Controllers.FriendsController>();
            //_container.RegisterType<TooDooWebRole.Controllers.HomeController>();

            //DependencyResolver.SetResolver(new IoCContainer(_container));
            DbConfiguration.SetConfiguration(new TooDooSvc.Persistence.EFConfiguration());
        }

        

    }
}