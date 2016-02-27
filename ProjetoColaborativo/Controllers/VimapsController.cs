using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetoColaborativo.Models.DAO;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Controllers
{
    public class VimapsController : Controller
    {
        private readonly IRepositorio<SessaoColaborativa> _repositorioSessaoColaborativa;

        public VimapsController(IRepositorio<SessaoColaborativa> repositorioSessaoColaborativa)
        {
            this._repositorioSessaoColaborativa = repositorioSessaoColaborativa;
        }

        [HttpPost]
        public ActionResult SendImage(string imgdata)
        {
            // Saving
            if (!string.IsNullOrEmpty(imgdata))
            {
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
            }

            return RedirectToAction("ShowSession");
        }

        [Authorize]
        public ActionResult ShowSession(int? sessionid)
        {
            SessaoColaborativa sessao = _repositorioSessaoColaborativa.RetornarTodos().FirstOrDefault();
            if (sessao == null)
            {
                sessao = new SessaoColaborativa()
                {
                    Descricao = "teste"
                };
                _repositorioSessaoColaborativa.Incluir(sessao);
            }

            ViewBag.UploadedImageUrl = TempData["ThumbImageSavedURL"];
            return View();
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();

            foreach (var codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;
            
            return null;
        }
    }
}