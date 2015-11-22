using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using ProjetoColaborativo.Models.Entidades.Mappings;

namespace ProjetoColaborativo.Models.DAO
{
    public class SessionHelper
    {
        private static ISessionFactory sessionFactory;
        public static ISessionFactory BuildSessionFactory()
        {
            // Se a sessão já existir, retorná-la
            if (sessionFactory != null)
                return sessionFactory;

            var configuration = CreateNewConfiguration();

            sessionFactory = configuration.BuildSessionFactory();

            return sessionFactory;
        }

        private static Configuration CreateNewConfiguration()
        {
            const string connectionString = "Data Source = localhost; Initial Catalog = projetoColaborativo; Integrated Security = True";

            var configuration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString).ShowSql)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UsuarioMapping>())
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                .BuildConfiguration();

            return configuration;
        }

        public static ISession GetCurrentSession()
        {
            return BuildSessionFactory().OpenSession();
        }
    }
}
