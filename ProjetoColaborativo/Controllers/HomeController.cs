using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
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
            Session["lastSessionId"] = null;
            Session["lastObjectId"] = null;

            ViewBag.NovoObjeto = TempData.Peek("ThumbImageTNSavedURL");

            var usuario = _repositorioUsuarios.Retornar(Convert.ToInt64(User.Identity.GetUserId()));

            var minhassessoes = _repositorioSessaoColaborativa
                    .Consultar(x =>
                        x.Usuario == usuario // minhas sessoes
                        && x.Fechada != true
                        && x.Arquivada == false
                    );

            var sessoesqueparticipo = _repositorioSessaoColaborativa
                    .Consultar(x =>
                        x.UsuariosDaSessao.Contains(usuario) // sessões que participo
                        && x.Arquivada == false
                        && x.Fechada != true
                    );

            var sessoesfechadas = _repositorioSessaoColaborativa
                    .Consultar(x =>
                        (
                            x.Usuario == usuario // minhas sessoes
                            ||
                            x.UsuariosDaSessao.Contains(usuario) // sessões que participo
                        )
                        && x.Fechada
                        && x.Arquivada == false
                    );

            ViewBag.MinhasSessoes = minhassessoes;
            ViewBag.SessoesFechadas = sessoesfechadas;
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