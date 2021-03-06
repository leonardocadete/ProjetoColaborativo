﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Hubs
{
    public class AtualizaElementos : Hub
    {
        public void Executar(SessaoColaborativa sessao)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<AtualizaElementos>();

            try
            {
                var usuariosDaSessao = new List<Usuario>(sessao.UsuariosDaSessao)
                {
                    sessao.Usuario
                };
                var usuariosLista = usuariosDaSessao.Select(x => x.Handle.ToString()).ToList();
                context.Clients.Users(usuariosLista).atualizar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}