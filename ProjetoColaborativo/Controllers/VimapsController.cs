using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
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
        public ActionResult SalvarElementoMultimidia(long id, long objetoid, Guid guid, string json, bool remover = false)
        {
            if (id == null)
                return RedirectToAction("EscolherSessao");

            var obj = _repositorioObjetosSessaoColaborativa.Retornar(objetoid);

            if (objetoid == null || obj == null)
                return RedirectToAction("EscolherSessao");

            var el = _repositorioElementoMultimidia.Consultar(x => x.Guid == guid).FirstOrDefault();
            var usuario = _repositorioUsuarios.Consultar(x => x.Nome.Equals(User.Identity.Name)).FirstOrDefault();

            if (el == null)
                el = new ElementoMultimidia
                {
                    Usuario = usuario,
                    Guid = guid,
                    Json = json
                };
            else
                el.Json = json;

            if (remover)
                obj.ElementosMultimidia.Remove(el);
            else
                obj.ElementosMultimidia.Add(el);

            _repositorioObjetosSessaoColaborativa.Salvar(obj);

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SalvarMiniatura(long id, long objetoid, string imgdata)
        {
            if (objetoid == null || string.IsNullOrEmpty(imgdata))
                return Json("error");

            var obj = _repositorioObjetosSessaoColaborativa.Retornar(objetoid);

            if (obj == null)
                return Json("error");

            var imagespath = Server.MapPath("~/UserData/Images");
            if (!Directory.Exists(imagespath))
                Directory.CreateDirectory(imagespath);

            var jpgEncoder = GetEncoder(ImageFormat.Png);
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, 90L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            var filename = obj.UrlMiniatura;
            var str64 = imgdata.Split(',')[1];
            var bytes = Convert.FromBase64String(str64);

            Image image;
            using (var ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            image.Save(imagespath + "/" + filename.Split('/')[filename.Split('/').Length - 1], jpgEncoder, myEncoderParameters);

            return Json("ok");
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
            var filenametn = DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + Guid.NewGuid() + "_tn.jpg";
            var str64 = imgdata.Split(',')[1];
            var bytes = Convert.FromBase64String(str64);

            Image image;
            using (var ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            image.Save(imagespath + "/" + filename, jpgEncoder, myEncoderParameters);
            image.Save(imagespath + "/" + filenametn, jpgEncoder, myEncoderParameters);
            TempData["ThumbImageSavedURL"] = "/UserData/Images/" + filename;
            TempData["ThumbImageTNSavedURL"] = "/UserData/Images/" + filenametn;

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

            var usuario = _repositorioUsuarios.Consultar(x => x.Nome.Equals(User.Identity.Name)).FirstOrDefault();

            ViewBag.LerElementos = "null";
            ViewBag.CorDono = usuario.Cor;

            if (obj.ElementosMultimidia.Count > 0)
            {
                List<string> els = new List<string>();
                foreach (var el in obj.ElementosMultimidia)
                {
                    Color cor = System.Drawing.ColorTranslator.FromHtml("#" + el.Usuario.Cor);
                    
                    string json = el.Json;
                    Regex regex = new Regex("fill:(.*)\"");
                    json = Regex.Replace(json, "(?<=fill\":\").*?(?=\")", string.Format("rgba({0}, {1}, {2}, 0.5)", cor.R, cor.G, cor.B));
                    json = Regex.Replace(json, "(?<=stroke\":\").*?(?=\")", string.Format("rgba({0}, {1}, {2}, 0.5)", cor.R, cor.G, cor.B));
                    els.Add(json);
                }
                ViewBag.LerElementos = string.Format("{{'objects': [ {0} ]}}", string.Join(",",els));
            }
            
            ViewBag.ObjectId = objetoid;
            return View(sessao);
        }

        [Authorize]
        public ActionResult EscolherSessao()
        {
            //TODO: pegar o usuario pelo handle
            var sessoes =
                _repositorioSessaoColaborativa.RetornarTodos()
                    .Where(x => x.Usuario.Nome.Equals(User.Identity.Name))
                    .Select(x => new { Handle = x.Handle, Descricao = x.Descricao + string.Format(" ({0})", x.Usuario.Nome) })
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
            var imgtn = TempData["ThumbImageTNSavedURL"];
            if (img == null)
                return RedirectToAction("MostrarSessao", "Vimaps", new {id = SessaoColaborativaId});

            sessao.ObjetosDaSessao.Add(new ObjetoSessao
            {
                UrlImagem = img.ToString(),
                UrlMiniatura = imgtn.ToString()
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