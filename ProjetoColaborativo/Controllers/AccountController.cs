using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ProjetoColaborativo.Models.Entidades;
using ProjetoColaborativo.ViewModels.Account;

namespace ProjetoColaborativo.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Usuario, string> signInManager;
        private readonly IAuthenticationManager authenticationManager;

        public AccountController( 
            IAuthenticationManager authenticationManager, 
            UserManager<Usuario> userManager)
        {
            this.authenticationManager = authenticationManager;
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
                        FormsAuthentication.SetAuthCookie(viewModel.Login, false);
                        return RedirectToLocal(returnUrl);
                    default:
                        ModelState.AddModelError("", "Usuário ou senha incorretos.");
                        return View(viewModel);
                }
            }

            ModelState.AddModelError("", "Usuário ou senha incorretos.");
            return View(viewModel);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            authenticationManager.SignOut();

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