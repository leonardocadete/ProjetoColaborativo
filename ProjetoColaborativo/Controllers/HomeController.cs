using System.Linq;
using System.Web.Mvc;
using ProjetoColaborativo.Models.DAO;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRepositorio<Usuario> _repositorioUsuarios;
        private readonly IRepositorio<SessaoColaborativa> _repositorioSessaoColaborativa;

        public HomeController(IRepositorio<Usuario> repositorioUsuarios,
                              IRepositorio<SessaoColaborativa> repositorioSessaoColaborativa)
        {
            this._repositorioUsuarios = repositorioUsuarios;
            this._repositorioSessaoColaborativa = repositorioSessaoColaborativa;
        }

        public ActionResult Index()
        {
            var usuario = _repositorioUsuarios.Consultar(x => x.Nome.Equals(User.Identity.Name)).FirstOrDefault();

            var minhassessoes = _repositorioSessaoColaborativa
                    .Consultar(x =>
                        x.Usuario == usuario // minhas sessoes
                    );

            var sessoesqueparticipo = _repositorioSessaoColaborativa
                    .Consultar(x =>
                        x.UsuariosDaSessao.Contains(usuario) // sessões que participo
                    );

            ViewBag.MinhasSessoes = minhassessoes;
            ViewBag.SessoesQueParticipo = sessoesqueparticipo;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}