using System;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using NHibernate;
using NHibernate.Context;
using ProjetoColaborativo.Models.DAO;

namespace ProjetoColaborativo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        internal static readonly IUnityContainer container = new UnityContainer();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Initialize unity container
            Bootstrapper.Initialise(container);
        }

        protected static ISessionFactory sessionFactory
        {
            get
            {
                return SessionHelper.BuildSessionFactory();
            }
        }

        // Evento executado toda vez que uma requisição inicia
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Context.Request.CurrentExecutionFilePathExtension != null &&
                !Context.Request.CurrentExecutionFilePathExtension.Equals(string.Empty))
                return;

            //var session = sessionFactory.OpenSession();
            var session = container.Resolve<ISessionFactory>().OpenSession();
            CurrentSessionContext.Bind(session);
            BeginTransaction(session);
        }

        // Evento executado toda vez que uma requisição finaliza
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            var session = CurrentSessionContext.Unbind(sessionFactory);
            if (session == null)
                return;
            try
            {
                session.Flush();
                if (session.Transaction.IsActive)
                {
                    if (HttpContext.Current.Server.GetLastError() == null)
                    {
                        try
                        {
                            session.Transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            var destino = Request.UrlReferrer.AbsoluteUri;

                            if (!destino.Contains("ConstraintViolation"))
                                destino += (destino.Contains("?") ? "&" : "?") + "ConstraintViolation=true";

                            Response.Redirect(destino);

                            session.Transaction.Rollback();
                        }
                    }
                    else
                    {
                        session.Transaction.Rollback();
                    }
                }
            }
            finally
            {
                Transaction.Current = null;
                session.Dispose();
            }
            //if (!this.Request.IsAuthenticated && this.IsAjaxRequest())
            //{
            //    this.Response.TrySkipIisCustomErrors = true;
            //    this.Response.ClearContent();
            //    this.Response.StatusCode = 401;
            //    this.Response.RedirectLocation = null;
            //}
        }

        /// <summary>
        /// Inicia uma transação
        /// </summary>
        /// <param name="session">Sessão do nHibernate</param>
        protected static void BeginTransaction(ISession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        }
    }
}
