using System.Web.Mvc;

namespace ProjetoColaborativo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITeste teste;

        public HomeController(ITeste teste)
        {
            this.teste = teste;
        }

        public ActionResult Index()
        {
            teste.Testar();
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