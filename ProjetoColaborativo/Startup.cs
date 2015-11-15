using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjetoColaborativo.Startup))]
namespace ProjetoColaborativo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
