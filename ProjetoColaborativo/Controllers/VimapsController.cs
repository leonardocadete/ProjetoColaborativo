using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ProjetoColaborativo.Models.DAO;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Controllers
{
    public class VimapsController : Controller
    {
        private readonly IRepositorio<Usuario> _repositorioUsuarios;
        private readonly IRepositorio<SessaoColaborativa> _repositorioSessaoColaborativa;
        private readonly IRepositorio<ObjetoSessao> _repositorioObjetosSessaoColaborativa;
        private readonly IRepositorio<ElementoMultimidia> _repositorioElementoMultimidia;

        public VimapsController(IRepositorio<Usuario> repositorioUsuarios,
                                IRepositorio<SessaoColaborativa> repositorioSessaoColaborativa,
                                IRepositorio<ObjetoSessao> repositorioObjetosSessaoColaborativa,
                                IRepositorio<ElementoMultimidia> repositorioElementoMultimidia)
        {
            this._repositorioUsuarios = repositorioUsuarios;
            this._repositorioSessaoColaborativa = repositorioSessaoColaborativa;
            this._repositorioObjetosSessaoColaborativa = repositorioObjetosSessaoColaborativa;
            this._repositorioElementoMultimidia = repositorioElementoMultimidia;
        }

        [HttpPost]
        [Authorize]
        [ActionName("MostrarSessao")]
        public ActionResult SalvarElementoMultimidia(long? id, long? objetoid, Guid guid, string json, bool remover = false)
        {
            if (id == null)
                return RedirectToAction("EscolherSessao");

            if (objetoid == null || _repositorioObjetosSessaoColaborativa.Retornar(objetoid.Value) == null)
                return RedirectToAction("EscolherSessao");

            var el = _repositorioElementoMultimidia.Consultar(x => x.Guid == guid).FirstOrDefault();

            if (el == null)
                el = new ElementoMultimidia
                {
                    Guid = guid,
                    Json = json
                };
            else
                el.Json = json;
            
            if(remover)
                _repositorioElementoMultimidia.Excluir(el);
            else
                _repositorioElementoMultimidia.Salvar(el);

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendImage(string imgdata)
        {
            // Saving
            if (string.IsNullOrEmpty(imgdata))
                return RedirectToAction("MostrarSessao");

            var imagespath = Server.MapPath("~/UserData/Images");
            if (!Directory.Exists(imagespath))
                Directory.CreateDirectory(imagespath);

            var jpgEncoder = GetEncoder(ImageFormat.Png);
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, 90L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            var filename = DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + Guid.NewGuid() + ".jpg";
            var str64 = imgdata.Split(',')[1];
            var bytes = Convert.FromBase64String(str64);

            Image image;
            using (var ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            image.Save(imagespath + "/" + filename, jpgEncoder, myEncoderParameters);
            TempData["ThumbImageSavedURL"] = "/UserData/Images/" + filename;

            return RedirectToAction("MostrarSessao");
        }

        [Authorize]
        public ActionResult MostrarSessao(long? id, long? objetoid)
        {
            if (id == null)
                return RedirectToAction("EscolherSessao");

            var sessao = _repositorioSessaoColaborativa.Retornar(id.Value);
            if (sessao == null)
                return RedirectToAction("EscolherSessao");

            var obj = sessao.ObjetosDaSessao.FirstOrDefault(x => x.Handle == objetoid);
            
            if (obj == null)
                return RedirectToAction("MostrarSessao", new { id = id, objetoid = sessao.ObjetosDaSessao.FirstOrDefault().Handle });
            
            if (obj.ElementosMultimidia.Count > 0)
            {
                var els = obj.ElementosMultimidia.Select(x => x.Json).ToList();
                ViewBag.LerElementos = string.Format("{{'objects': [ {0} ]}}", string.Join(",",els));
            }
            else
                ViewBag.LerElementos = "null";
            
            ViewBag.ObjectId = objetoid;
            return View(sessao);
        }

        [Authorize]
        public ActionResult EscolherSessao()
        {
            //TODO: pegar o usuario pelo handle
            List<SessaoColaborativa> sessoes =
                _repositorioSessaoColaborativa.RetornarTodos()
                    .Where(x => x.Usuario.Nome.Equals(User.Identity.Name))
                    .ToList();

            ViewBag.TemSessoes = sessoes.Count > 0;
            ViewBag.SessaoColaborativaId = new SelectList(
                sessoes,
                "Handle",
                "Descricao"
            );

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult EscolherSessao(string SessaoColaborativaId)
        {
            if (string.IsNullOrEmpty(SessaoColaborativaId))
                return View();

            var sessao = _repositorioSessaoColaborativa.Retornar(long.Parse(SessaoColaborativaId));
            if (sessao == null) return View();

            var img = TempData["ThumbImageSavedURL"];
            if (img == null)
                return RedirectToAction("MostrarSessao", "Vimaps", new {id = SessaoColaborativaId});

            sessao.ObjetosDaSessao.Add(new ObjetoSessao
            {
                UrlImagem = img.ToString()
            });
            _repositorioSessaoColaborativa.Salvar(sessao);

            return RedirectToAction("MostrarSessao", "Vimaps", new { id = SessaoColaborativaId });
        }

        [Authorize]
        [HttpPost]
        public ActionResult CriarSessaoColaborativa(string descricao)
        {
            //TODO: pegar usuario pelo handle
            var usuario = _repositorioUsuarios.RetornarTodos().FirstOrDefault(x => x.Login.Equals(User.Identity.Name));

            if (string.IsNullOrEmpty(descricao))
                return View("EscolherSessao", usuario);

            var sessao = new SessaoColaborativa
            {
                Usuario = usuario,
                Descricao = descricao
            };

            var img = TempData["ThumbImageSavedURL"];
            if (img != null)
            {
                sessao.ObjetosDaSessao.Add(new ObjetoSessao
                {
                    UrlImagem = img.ToString()
                });
            }

            _repositorioSessaoColaborativa.Salvar(sessao);
            return RedirectToAction("MostrarSessao", "Vimaps", new { id = sessao.Handle });
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();

            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }
}