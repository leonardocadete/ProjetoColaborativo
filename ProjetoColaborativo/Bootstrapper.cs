using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using NHibernate;
using ProjetoColaborativo.Business.Extensions;
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

            RegisterRepositorios(container);

            container.RegisterType<ITeste, Teste>();
        }

        private static void RegisterRepositorios(IUnityContainer container)
        {
            // Todas as classes que herdam de entidade
            var fullList = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.IsClassOrSubclassOf<Entidade>()));

            // Regitro automático
            foreach (var repo in fullList)
            {
                var from = typeof(IRepositorio<>).MakeGenericType(repo);
                var to = typeof(Repositorio<>).MakeGenericType(repo);

                container.RegisterType(from, to);
            }
        }
    }
}