using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using ProjetoColaborativo.Models.DAO;
using ProjetoColaborativo.Models.Entidades;
using ProjetoColaborativo.ViewModels.Usuario;

namespace ProjetoColaborativo.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IRepositorio<Usuario> repositorioUsuario;

        public UsuarioController(IRepositorio<Usuario> repositorioUsuario)
        {
            this.repositorioUsuario = repositorioUsuario;
        }

        public ActionResult Index()
        {
            var usuarios = repositorioUsuario.RetornarTodos();
            var usuariosViewModel = Mapper.Map<IList<UsuarioViewModel>>(usuarios);
            return View(usuariosViewModel);
        }

        public ActionResult Create(long id)
        {
            return RedirectToAction("Index");
        }

        public ActionResult Delete(long id)
        {
            return RedirectToAction("Index");
        }
    }
}