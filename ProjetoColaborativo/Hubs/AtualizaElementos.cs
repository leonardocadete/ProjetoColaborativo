using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Hubs
{
    public class AtualizaElementos : Hub
    {
        public void Executar(IList<Usuario> usuarios)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<AtualizaElementos>();

            try
            {
                var usuariosLista = usuarios.Select(x => x.Handle.ToString()).ToList();

                context.Clients.Users(usuariosLista).atualizar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}