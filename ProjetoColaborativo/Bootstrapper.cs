using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using NHibernate;
using ProjetoColaborativo.Controllers;
using ProjetoColaborativo.Models.DAO;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo
{
    public class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here  
            //This is the important line to edit  
            RegisterTypes(container);
            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<ISessionFactory>(new InjectionFactory(c => SessionHelper.BuildSessionFactory()));
            container.RegisterType<ISession>(new InjectionFactory(c => SessionHelper.GetCurrentSession()));

            container.RegisterType<IRepositorio<Usuario>, Repositorio<Usuario>>();

            container.RegisterType<ITeste, Teste>();
        }
    }
}