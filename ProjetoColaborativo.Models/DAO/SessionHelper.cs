﻿using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
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
            //const string connectionString = "Data Source=|DataDirectory|Colaborativo.sdf;Persist Security Info=False;";
            
            var configuration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString).ShowSql)
                //.Database(MsSqlCeConfiguration.MsSqlCe40.ConnectionString(connectionString).ShowSql)
                .Mappings(
                    m =>
                    {
                        m.FluentMappings.AddFromAssemblyOf<UsuarioMapping>();
                        m.FluentMappings.AddFromAssemblyOf<SessaoColaborativaMapping>();
                        m.FluentMappings.AddFromAssemblyOf<ObjetoSessaoMapping>();
                        m.FluentMappings.AddFromAssemblyOf<ElementoMultimidiaMapping>();
                        m.FluentMappings.AddFromAssemblyOf<NotificacaoMapping>();
                    }
                 )
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                .ExposeConfiguration(cfg => cfg.SetProperty(Environment.CurrentSessionContextClass,"web"))
                .BuildConfiguration();

            return configuration;
        }

        public static ISession GetCurrentSession()
        {
            if(!CurrentSessionContext.HasBind(BuildSessionFactory()))
            CurrentSessionContext.Bind(BuildSessionFactory().OpenSession());

            return BuildSessionFactory().GetCurrentSession();
        }
    }
}
