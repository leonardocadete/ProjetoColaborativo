using System.Linq;
using ProjetoColaborativo.Models.DAO;

namespace ProjetoColaborativo.Business.Cargas
{
    public class CargaUsuarioAdm : ICargaUsuarioAdm
    {
        private const string loginAdm = "admin";
        private readonly IRepositorio<Models.Entidades.Usuario> repositorio;

        public CargaUsuarioAdm(IRepositorio<Models.Entidades.Usuario> repositorio)
        {
            this.repositorio = repositorio;
        }

        public void Executar()
        {
            var userAdm = repositorio.Consultar(x => x.Login == loginAdm).FirstOrDefault();

            if (userAdm != null) return;
            var user = new Models.Entidades.Usuario
            {
                Nome = "Administrador",
                Login = "admin",
                Senha = "ANx7PRdrECUZsUsYYjpAjDpD4KH9mM9J+cjCE97JeshuUf4hCG+u9ND1/M6XMMHAZQ==" // 1q2w#E
            };
            repositorio.Salvar(user);
        }
    }
}
