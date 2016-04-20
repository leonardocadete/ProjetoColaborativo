using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ProjetoColaborativo.Business.Cargas;
using ProjetoColaborativo.Models.Entidades;
using ProjetoColaborativo.ViewModels.Account;

namespace ProjetoColaborativo.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Usuario, string> signInManager;
        private readonly IAuthenticationManager authenticationManager;
        private readonly ICargaUsuarioAdm cargaUsuarioAdm;

        public AccountController( 
            IAuthenticationManager authenticationManager, 
            UserManager<Usuario> userManager, 
            ICargaUsuarioAdm cargaUsuarioAdm)
        {
            this.authenticationManager = authenticationManager;
            this.cargaUsuarioAdm = cargaUsuarioAdm;
            this.signInManager = new SignInManager<Usuario, string>(userManager, authenticationManager);
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
        public async Task<ActionResult> Login(LoginViewModel viewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await signInManager.PasswordSignInAsync(viewModel.Login, viewModel.Senha, true, true);
                switch (loginResult)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);
                    default:
                        ModelState.AddModelError("", "Usuário ou senha incorretos.");
                        return View(viewModel);
                }
            }

            ModelState.AddModelError("", "Usuário ou senha incorretos.");
            return View(viewModel);
        }

        [Authorize]
        public ActionResult LogOff()
        {
            authenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult CargaUsuarioAdm()
        {
            cargaUsuarioAdm.Executar();
            return RedirectToAction("Login");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}