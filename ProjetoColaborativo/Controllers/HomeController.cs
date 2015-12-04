using System.Linq;
using System.Web.Mvc;
using ProjetoColaborativo.Models.DAO;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepositorio<Usuario> repositorioUsuario; 

        public HomeController(IRepositorio<Usuario> repositorioUsuario)
        {
            this.repositorioUsuario = repositorioUsuario;
        }

        public ActionResult Index()
        {
            var usuarios = repositorioUsuario.RetornarTodos().First();
            usuarios.Nome = "Alterado";
            repositorioUsuario.Salvar(usuarios);
            //repositorioUsuario.Sessao.Flush();
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