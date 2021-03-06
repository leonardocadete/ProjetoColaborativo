﻿using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using ProjetoColaborativo.Hubs;

[assembly: OwinStartupAttribute(typeof(ProjetoColaborativo.Startup))]
namespace ProjetoColaborativo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.MapSignalR();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new SignalRUserIdProvider());
        }
    }
}
