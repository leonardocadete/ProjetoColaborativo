using System.Collections.Generic;
using ProjetoColaborativo.Business.Usuario.ViewModels;

namespace ProjetoColaborativo.Business.Usuario
{
    public interface IRepositorioUsuario
    {
        IEnumerable<UsuarioViewModel> ObterUsuarios(string q, long handleUsuarioLogado = 0);
        UsuarioViewModel RetornarUsuario(long handle);
        void ExcluirUsuario(long handle);
    }
}
