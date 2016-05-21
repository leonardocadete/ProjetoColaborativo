using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ProjetoColaborativo.Business.Usuario.ViewModels;
using ProjetoColaborativo.Models.DAO;

namespace ProjetoColaborativo.Business.Usuario
{
    public class RepositorioUsuario : IRepositorioUsuario
    {
        private readonly IRepositorio<Models.Entidades.Usuario> repositorio;

        public RepositorioUsuario(IRepositorio<Models.Entidades.Usuario> repositorio)
        {
            this.repositorio = repositorio;
        }

        public IEnumerable<UsuarioViewModel> ObterUsuarios(string q)
        {
            var usuarios = repositorio.RetornarTodos();
            if (q != null)
                usuarios = usuarios.Where(x => x.Nome.Contains(q));
            var usuariosViewModel = Mapper.Map<IList<UsuarioViewModel>>(usuarios);
            return usuariosViewModel;
        }

        public UsuarioViewModel RetornarUsuario(long handle)
        {
            var usuario = repositorio.Retornar(handle) ?? new Models.Entidades.Usuario();
            var usuarioViewModel = Mapper.Map<UsuarioViewModel>(usuario);
            return usuarioViewModel;
        }

        public void ExcluirUsuario(long handle)
        {
            var usuario = repositorio.Retornar(handle);
            repositorio.Excluir(usuario);
        }
    }
}
