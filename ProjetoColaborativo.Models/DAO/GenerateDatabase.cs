using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using ProjetoColaborativo.Models.Entidades.Mappings;

namespace ProjetoColaborativo.Models.DAO
{
    public static class GenerateDatabase
    {
        public static void Generate()
        {
            const string connectionString = "Data Source = localhost; Initial Catalog = projetoColaborativo; Integrated Security = True";
            
            var configuration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString).ShowSql)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UsuarioMapping>())
                .BuildConfiguration();

            var exporter = new SchemaExport(configuration);
            exporter.Execute(true, true, false);
        }
    }
}
