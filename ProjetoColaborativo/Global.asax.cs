using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using ProjetoColaborativo.Controllers;

namespace ProjetoColaborativo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public IUnityContainer container;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            container = new UnityContainer();
            container.RegisterType<ITeste, Teste>();
        }
    }
}
