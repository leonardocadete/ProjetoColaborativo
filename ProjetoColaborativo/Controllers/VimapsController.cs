﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ProjetoColaborativo.Hubs;
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
        [ActionName("MostrarSessaoIframe")]
        public ActionResult SalvarElementoMultimidia(long id, long objetoid, Guid guid, string json, bool remover = false)
        {
            var obj = _repositorioObjetosSessaoColaborativa.Retornar(objetoid);

            if (obj == null)
                return RedirectToAction("EscolherSessao");

            var el = _repositorioElementoMultimidia.Consultar(x => x.Guid == guid).FirstOrDefault();
            var usuario = _repositorioUsuarios.Retornar(Convert.ToInt64(User.Identity.GetUserId()));

            json = json.Replace("\n", "\\n");
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

            var sessao = _repositorioSessaoColaborativa.Retornar(id);
            var atualizaElementosHub = new AtualizaElementos();
            atualizaElementosHub.Executar(sessao);

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult GetDadosUsuario(long idusuario)
        {
            var u = _repositorioUsuarios.Retornar(idusuario);
            if (u != null)
                return Json(new
                {
                    u.Id,
                    u.Email,
                    u.Login,
                    u.Nome,
                    u.Foto
                }, JsonRequestBehavior.AllowGet);

            return null;
        }

        [HttpPost]
        [Authorize]
        public ActionResult OrdenarObjeto(long id, long objetoid, long idanterior, long idreordenar)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);

            if (sessao == null)
                return Json("", JsonRequestBehavior.AllowGet);

            int ordematual = 1;

            if (idanterior == 0)
            {
                sessao.ObjetosDaSessao.FirstOrDefault(x => x.Handle == idreordenar).Ordem = 1;
                ordematual++;
            }

            foreach (var objetoSessao in sessao.ObjetosDaSessao.OrderBy(x => x.Ordem))
            {
                if (objetoSessao.Handle == idreordenar)
                    continue;

                objetoSessao.Ordem = ordematual;

                ordematual++;

                if (objetoSessao.Handle == idanterior)
                {
                    sessao.ObjetosDaSessao.FirstOrDefault(x => x.Handle == idreordenar).Ordem = ordematual;
                    ordematual++;
                }
            }

            _repositorioSessaoColaborativa.Salvar(sessao);

            var atualizaElementosHub = new AtualizaElementos();
            atualizaElementosHub.Executar(sessao);

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SalvarMiniatura(long id, long objetoid, string imgdata)
        {
            if (string.IsNullOrEmpty(imgdata))
                return Json("error");

            var obj = _repositorioObjetosSessaoColaborativa.Retornar(objetoid);

            if (obj == null)
                return Json("error");

            var imagespath = Server.MapPath("~/UserData/Images");
            var filename = obj.UrlMiniatura;
            if (!Directory.Exists(imagespath))
                Directory.CreateDirectory(imagespath);

            byte[] bitmapData = new byte[imgdata.Length];
            bitmapData = Convert.FromBase64String(imgdata.Split(',')[1]);

            Image image;
            using (var streamBitmap = new MemoryStream(bitmapData))
            {
                using (image = Image.FromStream(streamBitmap))
                {
                    image.Save(imagespath + "/" + filename.Split('/')[filename.Split('/').Length - 1]);
                }
            }

            return Json("ok");
        }

        public ActionResult SendAudio(HttpPostedFileBase file, string objectid)
        {
            var audiospath = Server.MapPath("~/UserData/Audio");
            if (!Directory.Exists(audiospath))
                Directory.CreateDirectory(audiospath);

            file.SaveAs(audiospath + "//" + objectid + ".wav");

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendVideo(HttpPostedFileBase file, string objectid)
        {
            var videospath = Server.MapPath("~/UserData/Video");
            if (!Directory.Exists(videospath))
                Directory.CreateDirectory(videospath);

            file.SaveAs(videospath + "//" + objectid + ".webm");

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendImage(string imgdata, string url)
        {
            // Saving
            if (string.IsNullOrEmpty(imgdata))
                return RedirectToAction("MostrarSessao");

            var imagespath = Server.MapPath("~/UserData/Images");
            if (!Directory.Exists(imagespath))
                Directory.CreateDirectory(imagespath);

            var jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, 90L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            var myEncodert = Encoder.Quality;
            var myEncoderParameterst = new EncoderParameters(1);
            var myEncoderParametert = new EncoderParameter(myEncodert, 10L);
            myEncoderParameterst.Param[0] = myEncoderParametert;

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
            image.Save(imagespath + "/" + filenametn, jpgEncoder, myEncoderParameterst);

            // VERIFICANDO SESSÃO ABERTA
            if (Session["lastSessionId"] != null && Session["lastObjectId"] != null)
            {
                long sessionid, objectid;

                if (long.TryParse(Session["lastSessionId"].ToString(), out sessionid) &&
                    long.TryParse(Session["lastObjectId"].ToString(), out objectid))
                {
                    var sessao = _repositorioSessaoColaborativa.Retornar(sessionid);

                    int ordem = 1;

                    if (sessao.ObjetosDaSessao.Count > 0)
                        ordem = sessao.ObjetosDaSessao.Max(x => x.Ordem) + 1;

                    var usuario = _repositorioUsuarios.Consultar(x => x.Handle.Equals(User.Identity.GetUserId<long>())).FirstOrDefault();

                    var objeto = new ObjetoSessao
                    {
                        UrlImagem = "/UserData/Images/" + filename,
                        UrlMiniatura = "/UserData/Images/" + filenametn,
                        Ordem = ordem,
                        UrlOrigem = url.ToString(),
                        Usuario = usuario
                    };

                    sessao.ObjetosDaSessao.Add(objeto);
                    sessao = _repositorioSessaoColaborativa.Salvar(sessao);

                    // COLOCANDO OBJETO DEPOIS DO OBJETO ATUAL
                    objeto = sessao.ObjetosDaSessao.FirstOrDefault(x => x.Ordem == ordem);
                    long ido = 0;
                    if (Session["lastObjectId"] != null && long.TryParse(Session["lastObjectId"].ToString(), out ido))
                        OrdenarObjeto(sessao.Handle, objeto.Handle, ido, objeto.Handle);

                    return RedirectToAction("MostrarSessao", "Vimaps", new { id = sessao.Handle, objetoid = objeto.Handle });
                }
            }

            TempData["ThumbImageSavedURL"] = "/UserData/Images/" + filename;
            TempData["ThumbImageTNSavedURL"] = "/UserData/Images/" + filenametn;
            TempData["UrlReferer"] = url;

            return RedirectToAction("MostrarSessao");

        }

        [Authorize]
        public ActionResult ExcluirObjetoDaSessao(long id, long objetoid)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);
            var usuario = _repositorioUsuarios.Retornar(User.Identity.GetUserId<long>());
            var obj = sessao.ObjetosDaSessao.FirstOrDefault(x => x.Handle == objetoid);

            if (obj.Usuario.Handle == usuario.Handle)
            {
                sessao.ObjetosDaSessao.Remove(obj);
            }

            if (sessao.ObjetosDaSessao.Count > 0)
                return RedirectToAction("MostrarSessao", new { id = sessao.Handle, objetoid = sessao.ObjetosDaSessao.FirstOrDefault().Handle });

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ExcluirSessao(long id)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);
            var usuario = _repositorioUsuarios.Retornar(User.Identity.GetUserId<long>());

            if (sessao.Usuario.Handle == usuario.Handle)
            {
                _repositorioSessaoColaborativa.Excluir(sessao);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public ActionResult RenomearSessao(long id, string nome)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);
            var usuario = _repositorioUsuarios.Retornar(User.Identity.GetUserId<long>());

            if (sessao.Usuario.Handle == usuario.Handle && !string.IsNullOrEmpty(nome))
            {
                sessao.Descricao = nome;
                return Json(id, JsonRequestBehavior.AllowGet);
            }

            return Json("-1", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult FecharSessao(long id)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);
            var usuario = _repositorioUsuarios.Retornar(User.Identity.GetUserId<long>());

            if (sessao.Usuario.Handle == usuario.Handle)
            {
                sessao.Fechada = true;
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult AbrirSessao(long id)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);
            var usuario = _repositorioUsuarios.Retornar(User.Identity.GetUserId<long>());

            if (sessao.Usuario.Handle == usuario.Handle)
            {
                sessao.Fechada = false;
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ArquivarSessao(long id)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);
            var usuario = _repositorioUsuarios.Retornar(User.Identity.GetUserId<long>());

            if (sessao.Usuario.Handle == usuario.Handle)
            {
                sessao.Arquivada = true;
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult DesarquivarSessao(long id)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);
            var usuario = _repositorioUsuarios.Retornar(User.Identity.GetUserId<long>());

            if (sessao.Usuario.Handle == usuario.Handle)
            {
                sessao.Arquivada = false;
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult BuscarElementosDosOutrosParticipantesJson(long id, long objetoid)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);

            if (sessao == null)
                return Json("", JsonRequestBehavior.AllowGet);

            var obj = sessao.ObjetosDaSessao.FirstOrDefault(x => x.Handle == objetoid);

            if (obj == null)
                return Json("", JsonRequestBehavior.AllowGet);

            var usuario = _repositorioUsuarios.Retornar(User.Identity.GetUserId<long>());

            List<ElementoMultimidia> elementosdosoutros =
                obj.ElementosMultimidia.Where(x => x.Usuario != usuario).ToList();

            List<string> els = new List<string>();
            if (elementosdosoutros.Count > 0)
            {
                foreach (var el in elementosdosoutros)
                {
                    Color cor = System.Drawing.ColorTranslator.FromHtml("#" + el.Usuario.Cor);

                    string json = el.Json;
                    if (json.Contains("\"type\":\"i-text\"")) // TEXTO
                    {
                        json = Regex.Replace(json, "(?<=fill\":\").*?(?=\")", "rgba(255, 255, 255, 1)");
                        json = Regex.Replace(json, "(?<=stroke\":\").*?(?=\")", "rgba(0, 0, 0, 1)");
                        json = Regex.Replace(json, "(?<=iddono\":\").*?(?=\")", el.Usuario.Id);
                        json = Regex.Replace(json, "(?<=textBackgroundColor\":\").*?(?=\")",
                            string.Format("rgba({0}, {1}, {2}, 0.5)", cor.R, cor.G, cor.B));
                        //json = json.Replace("\\n", "\n");
                    }
                    else // GEOMETRIA
                    {
                        json = Regex.Replace(json, "(?<=iddono\":\").*?(?=\")", el.Usuario.Id);
                        json = Regex.Replace(json, "(?<=fill\":\").*?(?=\")",
                            string.Format("rgba({0}, {1}, {2}, 0.5)", cor.R, cor.G, cor.B));
                        json = Regex.Replace(json, "(?<=stroke\":\").*?(?=\")",
                            string.Format("rgba({0}, {1}, {2}, 0.5)", cor.R, cor.G, cor.B));
                    }

                    els.Add(json);
                }
            }

            // orders
            var objs = sessao.ObjetosDaSessao.OrderBy(x => x.Ordem).ToList();

            if (objs.Count == 0)
                return Json("", JsonRequestBehavior.AllowGet);

            List<string> orders = new List<string>();
            foreach (var o in objs)
                orders.Add(string.Format("{{ \"id\":\"{0}\", \"order\":\"{1}\" }}", o.Handle, o.Ordem));

            return this.Content(string.Format("{{\"multimediaelements\": [ {0} ], \"objects\": [ {1} ]}}",
                string.Join(",", els),
                string.Join(",", orders)),
                "application/json");
        }

        [Authorize]
        public ActionResult BuscarUsuarios(long id, string term)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);
            if (sessao == null)
                return null;

            var usuarios = _repositorioUsuarios.Consultar(x => x.Nome.StartsWith(term)
            && !sessao.UsuariosDaSessao.Contains(x)
            && x != sessao.Usuario)
            .Select(x => new { Id = x.Handle, Nome = x.Nome }).ToList();

            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdicionarUsuario(long? id, long? usuarioid)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id.Value);
            if (sessao == null)
                return null;

            var usuario = _repositorioUsuarios.Consultar(x => x.Handle.Equals(usuarioid.Value)).FirstOrDefault();
            if (usuario == null)
                return null;

            if (!sessao.UsuariosDaSessao.Contains(usuario))
                sessao.UsuariosDaSessao.Add(usuario);

            return Json("ok");
        }

        public ActionResult ExcluirUsuario(long? id, long? usuarioid)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id.Value);
            if (sessao == null)
                return null;

            var usuario = _repositorioUsuarios.Consultar(x => x.Handle.Equals(usuarioid.Value)).FirstOrDefault();
            if (usuario == null)
                return null;

            if (sessao.UsuariosDaSessao.Contains(usuario))
                sessao.UsuariosDaSessao.Remove(usuario);

            return Json("ok");
        }

        public ActionResult GetListaDeUsuariosDaSessao(long id)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);
            if (sessao == null)
                return null;
            return PartialView("_ListaUsuariosDaSessao", sessao);
        }

        public ActionResult GetListaDeUsuariosDaSessaoHome(long id)
        {
            var sessao = _repositorioSessaoColaborativa.Retornar(id);
            if (sessao == null)
                return null;
            return PartialView("_ListaUsuariosDaSessao", sessao);
        }


        [Authorize]
        [RequiresSSL]
        public ActionResult MostrarSessaoIframe(long? id, long? objetoid)
        {
            if (id == null)
                return RedirectToAction("EscolherSessao");

            var sessao = _repositorioSessaoColaborativa.Retornar(id.Value);
            if (sessao == null)
                return RedirectToAction("EscolherSessao");

            var obj = sessao.ObjetosDaSessao.FirstOrDefault(x => x.Handle == objetoid);

            if (obj == null && sessao.ObjetosDaSessao.Count > 0)
                return RedirectToAction("MostrarSessao", new { id = id, objetoid = sessao.ObjetosDaSessao.FirstOrDefault().Handle });

            var usuario = _repositorioUsuarios.Consultar(x => x.Handle.Equals(User.Identity.GetUserId<long>())).FirstOrDefault();

            ViewBag.LerElementos = "null";
            ViewBag.Dono = usuario.Handle;
            ViewBag.CorDono = usuario.Cor;
            ViewBag.NovoObjeto = TempData["NovoObjeto"];
            ViewBag.Aberta = sessao.Fechada == false;
            TempData["NovoObjeto"] = null;
            TempData["ThumbImageSavedURL"] = null;
            TempData["ThumbImageTNSavedURL"] = null;

            if (obj != null)
                ViewBag.DonoDoObjeto = Convert.ToInt64(User.Identity.GetUserId()) == obj.Usuario.Handle;

            if (obj != null && obj.ElementosMultimidia.Count > 0)
            {
                List<string> els = new List<string>();
                foreach (var el in obj.ElementosMultimidia)
                {
                    Color cor = System.Drawing.ColorTranslator.FromHtml("#" + el.Usuario.Cor);

                    string json = el.Json;
                    if (json.Contains("\"type\":\"i-text\"")) // TEXTO
                    {
                        json = Regex.Replace(json, "(?<=fill\":\").*?(?=\")", "rgba(255, 255, 255, 1)");
                        json = Regex.Replace(json, "(?<=stroke\":\").*?(?=\")", "rgba(0, 0, 0, 1)");
                        json = Regex.Replace(json, "(?<=iddono\":\").*?(?=\")", el.Usuario.Id);
                        json = Regex.Replace(json, "(?<=textBackgroundColor\":\").*?(?=\")",
                            string.Format("rgba({0}, {1}, {2}, 0.5)", cor.R, cor.G, cor.B));
                        //json = json.Replace("\\n", "\n");
                    }
                    else // GEOMETRIA
                    {
                        json = Regex.Replace(json, "(?<=iddono\":\").*?(?=\")", el.Usuario.Id);
                        json = Regex.Replace(json, "(?<=fill\":\").*?(?=\")",
                            string.Format("rgba({0}, {1}, {2}, 0.5)", cor.R, cor.G, cor.B));
                        json = Regex.Replace(json, "(?<=stroke\":\").*?(?=\")",
                            string.Format("rgba({0}, {1}, {2}, 0.5)", cor.R, cor.G, cor.B));
                    }

                    els.Add(json);
                }
                ViewBag.LerElementos = string.Format("{{'objects': [ {0} ]}}", string.Join(",", els));
            }

            ViewBag.ObjectId = objetoid;
            Session["lastObjectId"] = objetoid;
            Session["lastSessionId"] = sessao.Handle;
            return View(sessao);
        }

        [Authorize]
        [RequiresSSL]
        public ActionResult MostrarSessao(long? id, long? objetoid)
        {
            if (id == null)
                return RedirectToAction("EscolherSessao");

            var sessao = _repositorioSessaoColaborativa.Retornar(id.Value);
            if (sessao == null)
                return RedirectToAction("EscolherSessao");

            var obj = sessao.ObjetosDaSessao.FirstOrDefault(x => x.Handle == objetoid);

            if (obj == null && sessao.ObjetosDaSessao.Count > 0)
                return RedirectToAction("MostrarSessao", new { id = id, objetoid = sessao.ObjetosDaSessao.FirstOrDefault().Handle });

            ViewBag.CaminhoIframe = Url.Action("MostrarSessaoIframe", "Vimaps", new {id, objetoid});
            return View(sessao);
        }

        [Authorize]
        public ActionResult EscolherSessao(string SessaoColaborativaId)
        {
            if (string.IsNullOrEmpty(SessaoColaborativaId))
                return RedirectToAction("Index", "Home");

            var usuario = _repositorioUsuarios.Retornar(Convert.ToInt64(User.Identity.GetUserId()));

            if (string.IsNullOrEmpty(SessaoColaborativaId))
                return View();

            var sessao = _repositorioSessaoColaborativa.Retornar(long.Parse(SessaoColaborativaId));
            if (sessao == null) return View();

            var img = TempData["ThumbImageSavedURL"];
            TempData["ThumbImageSavedURL"] = null;
            var imgtn = TempData["ThumbImageTNSavedURL"];
            var url = TempData["UrlReferer"];
            if (img == null)
                return RedirectToAction("MostrarSessao", "Vimaps", new { id = SessaoColaborativaId });

            int ordem = 1;

            if (sessao.ObjetosDaSessao.Count > 0)
                ordem = sessao.ObjetosDaSessao.Max(x => x.Ordem) + 1;

            var objeto = new ObjetoSessao
            {
                UrlImagem = img.ToString(),
                UrlMiniatura = imgtn.ToString(),
                Ordem = ordem,
                UrlOrigem = url.ToString(),
                Usuario = usuario
            };

            sessao.ObjetosDaSessao.Add(objeto);
            sessao = _repositorioSessaoColaborativa.Salvar(sessao);

            // MOSTRANDO PRA ESCOLHER QUAL SESSÃO
            objeto = sessao.ObjetosDaSessao.FirstOrDefault(x => x.Ordem == ordem);
            if (sessao.ObjetosDaSessao.Count > 0)
                TempData["NovoObjeto"] = objeto.Handle;

            return RedirectToAction("MostrarSessao", "Vimaps", new { id = SessaoColaborativaId, objetoid = objeto.Handle });
        }

        [Authorize]
        [HttpPost]
        public ActionResult CriarSessaoColaborativa(string descricao)
        {
            var usuario = _repositorioUsuarios.Retornar(Convert.ToInt64(User.Identity.GetUserId()));

            if (string.IsNullOrEmpty(descricao))
                return View("EscolherSessao", usuario);

            var sessao = new SessaoColaborativa
            {
                Usuario = usuario,
                Descricao = descricao
            };

            var img = TempData["ThumbImageSavedURL"];
            TempData["ThumbImageSavedURL"] = null;
            var imgtn = TempData["ThumbImageTNSavedURL"];
            var url = TempData["UrlReferer"];
            if (img != null)
            {
                sessao.ObjetosDaSessao.Add(new ObjetoSessao
                {
                    UrlMiniatura = imgtn.ToString(),
                    UrlImagem = img.ToString(),
                    Ordem = 1,
                    UrlOrigem = url.ToString(),
                    Usuario = usuario
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