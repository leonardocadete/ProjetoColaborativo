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

        public ActionResult Create(long id = 0)
        {
            var usuario = repositorioUsuario.Retornar(id) ?? new Usuario();
            var usuarioViewModel = Mapper.Map<UsuarioViewModel>(usuario);
            return View(usuarioViewModel);
        }

        [HttpPost]
        public ActionResult Create(UsuarioViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var entidade = Mapper.Map<Usuario>(viewModel);
            repositorioUsuario.Salvar(entidade);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(long id)
        {
            var usuario = repositorioUsuario.Retornar(id);
            var usuarioViewModel = Mapper.Map<UsuarioViewModel>(usuario);
            return View(usuarioViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var usuario = repositorioUsuario.Retornar(id);
            repositorioUsuario.Excluir(usuario);
            return RedirectToAction("Index");
        }
    }
}