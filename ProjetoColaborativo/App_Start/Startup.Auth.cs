using System;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace ProjetoColaborativo
{
    public partial class Startup
    {
        public const int TEMPO_PARA_SESSAO_EXPIRAR_EM_MINUTOS = 20;

        public void ConfigureAuth(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logoff"),
                ExpireTimeSpan = TimeSpan.FromMinutes(TEMPO_PARA_SESSAO_EXPIRAR_EM_MINUTOS),
                SlidingExpiration = true
            });
        }
    }
}
