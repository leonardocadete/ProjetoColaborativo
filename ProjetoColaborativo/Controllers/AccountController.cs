using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using ProjetoColaborativo.Models.DAO;
using ProjetoColaborativo.Models.Entidades;
using ProjetoColaborativo.ViewModels.Account;

namespace ProjetoColaborativo.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepositorio<Usuario> repositorioUsuario;

        public AccountController(IRepositorio<Usuario> repositorioUsuario)
        {
            this.repositorioUsuario = repositorioUsuario;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel viewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var usuario =
                    repositorioUsuario.Consultar(x => x.Login == viewModel.Login && x.Senha == viewModel.Senha)
                        .FirstOrDefault();
                if (usuario != null)
                {
                    FormsAuthentication.SetAuthCookie(usuario.Login, false);

                    return RedirectToLocal(returnUrl);
                }
            }

            ModelState.AddModelError("", "Usuário ou senha incorretos.");
            return View(viewModel);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}