using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ProjetoColaborativo.Business.Usuario.ViewModels;
using ProjetoColaborativo.Models.DAO;

namespace ProjetoColaborativo.Business.Usuario
{
    public class RepositorioUsuario
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
    }
}
