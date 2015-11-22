using System.Linq;
using System.Web.Mvc;
using ProjetoColaborativo.Models.DAO;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITeste teste;
        private readonly IRepositorio<Usuario> repositorioUsuario; 

        public HomeController(ITeste teste, IRepositorio<Usuario> repositorioUsuario)
        {
            this.teste = teste;
            this.repositorioUsuario = repositorioUsuario;
        }

        public ActionResult Index()
        {
            teste.Testar();
            var usuarios = repositorioUsuario.RetornarTodos().First();
            usuarios.Nome = "Alterado";
            repositorioUsuario.Salvar(usuarios);
            repositorioUsuario.Sessao.Flush();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}