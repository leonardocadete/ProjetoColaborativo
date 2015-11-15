using Microsoft.Owin;
using Owin;
using ProjetoColaborativo.Models.DAO;

[assembly: OwinStartupAttribute(typeof(ProjetoColaborativo.Startup))]
namespace ProjetoColaborativo
{
    public partial class Startup
    {
        //ConfigureAuth(app);
        public void Configuration(IAppBuilder app)
        {
            GenerateDatabase.Generate();
        }
    }

}
