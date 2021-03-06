﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using NHibernate;
using ProjetoColaborativo.Business.Usuario;
using ProjetoColaborativo.Business.Usuario.ViewModels;
using ProjetoColaborativo.Models.DAO;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IRepositorio<Usuario> repositorioUsuario;
        private readonly UserManager<Usuario> userManager;
        private readonly ISession session;
        private readonly IRepositorioUsuario repositorioBusiness;

        public UsuarioController(
            IRepositorio<Usuario> repositorioUsuario, 
            UserManager<Usuario> userManager, 
            ISession session, 
            IRepositorioUsuario repositorioBusiness)
        {
            this.repositorioUsuario = repositorioUsuario;
            this.userManager = userManager;
            this.session = session;
            this.repositorioBusiness = repositorioBusiness;
        }

        public ActionResult Index(string q)
        {
            return View(repositorioBusiness.ObterUsuarios(q, Convert.ToInt64(User.Identity.GetUserId())));
        }

        public ActionResult Create(long id = 0)
        {
            return View(repositorioBusiness.RetornarUsuario(id));
        }

        [HttpPost]
        public async Task<ActionResult> Create(UsuarioViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var entidade = Mapper.Map<Usuario>(viewModel);

            if(string.IsNullOrEmpty(entidade.Cor))
                entidade.Cor = getRandomColorHex();

            if (!string.IsNullOrEmpty(entidade.Foto))
            {
                entidade.Foto = null;

                if (Request.Files.Count > 0)
                {
                    var imagespath = Server.MapPath("~/UserData/Profiles");

                    if (!Directory.Exists(imagespath))
                        Directory.CreateDirectory(imagespath);

                    var filename = DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + Guid.NewGuid() + ".jpg";
                    var file = Request.Files[0];

                    var validImageTypes = new string[]
                    {
                        "image/gif",
                        "image/jpeg",
                        "image/pjpeg",
                        "image/png"
                    };

                    if (!validImageTypes.Contains(file.ContentType))
                        ModelState.AddModelError("Foto", "Please choose either a GIF, JPG or PNG image.");
                    else
                    {
                        file.SaveAs(imagespath + "/" + filename);
                        entidade.Foto = "/UserData/Profiles/" + filename;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var result = await userManager.CreateAsync(entidade, entidade.Senha);
                if (!result.Succeeded)
                {
                    if (session.Transaction.IsActive)
                        session.Transaction.Rollback();
                    ModelState.AddModelError("Senha", "Senha deve conter no mínimo 6 caracteres");
                }
                else
                {
                    repositorioUsuario.Salvar(entidade);
                    return RedirectToAction("Index");
                }
            }
            
            return View("Create", viewModel);
        }

        public ActionResult Delete(long id)
        {
            return View(repositorioBusiness.RetornarUsuario(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            repositorioBusiness.ExcluirUsuario(id);
            return RedirectToAction("Index");
        }

        private string getRandomColorHex()
        {
            string[] cores =
            {
                 "2B2B2B",
                 "946D6C",
                 "9E373B",
                 "EEEEEE",
                 "4C8830",
                 "666666",
                 "587164",
                 "F3A929",
                 "BC3BFC",
                 "8F969C",
                 "1136C7",
                 "C97131",
                 "52070C",
                 "9A5C47",
                 "D4D6B9",
                 "706051",
                 "B75669",
                 "877D71",
                 "302F71",
                 "59503F",
                 "E0E4CC",
                 "D6DAC2",
                 "7BB0A6",
                 "92F22A",
                 "64DDBB",
                 "7CEECE",
                 "8F6F40",
                 "6F532A",
                 "523D1F",
                 "A0B58D",
                 "A19C69",
                 "FD5B03",
                 "63393E",
                 "3C3741",
                 "8C7E51",
                 "54573A",
                 "F04903",
                 "FF7416",
                 "F29B34",
                 "D33257",
                 "3D8EB9",
                 "71BA51",
                 "FEC606",
                 "E75926",
                 "EB6361",
                 "EBBD63",
                 "6C8784",
                 "45362E",
                 "87766C",
                 "25373D",
                 "CF000F",
                 "E3000E",
                 "E6DCDB",
                 "D2D7D3",
                 "E7E7E7",
                 "282830",
                 "BADA55",
                 "1DABB8",
                 "C82647",
                 "FF6766",
                 "E0E4CC",
                 "D6DAC2",
                 "7BB0A6",
                 "92F22A",
                 "64DDBB",
                 "7CEECE",
                 "8F6F40",
                 "6F532A",
                 "523D1F",
                 "A0B58D",
                 "A19C69",
                 "FD5B03",
                 "63393E",
                 "3C3741",
                 "8C7E51",
                 "54573A",
                 "F04903",
                 "FF7416",
                 "F29B34",
                 "D33257",
                 "3D8EB9",
                 "71BA51",
                 "FEC606",
                 "E75926",
                 "EB6361",
                 "EBBD63",
                 "6C8784",
                 "45362E",
                 "87766C",
                 "25373D",
                 "CF000F",
                 "E3000E",
                 "E6DCDB",
                 "D2D7D3",
                 "E7E7E7",
                 "282830",
                 "BADA55",
                 "1DABB8",
                 "C82647",
                 "FF6766",
                 "60646D",
                 "FFFFF7",
                 "83D6DE",
                 "97CE68",
                 "EB9532",
                 "EE543A",
                 "D8335B",
                 "953163",
                 "422E39",
                 "FACA9B",
                 "FDD09F",
                 "F3D89F",
                 "E7DF86",
                 "C0BA78",
                 "AAB69B",
                 "9E906E",
                 "9684A3",
                 "8870FF",
                 "888888",
                 "897FBA",
                 "8870FF",
                 "2C82C9",
                 "2CC990",
                 "EEE657",
                 "FCB941",
                 "FC6042",
                 "3E4651",
                 "F1654C",
                 "00B5B5",
                 "D4D4D4",
                 "D98B3A",
                 "D6523C",
                 "BB3658",
                 "7E3661",
                 "43353D",
                 "E3C39D",
                 "E26E67",
                 "E01931",
                 "8A2D3C",
                 "344146",
                 "EFE0B1",
                 "DBCB8E",
                 "9E9D9B",
                 "847858",
                 "9E8E5A",
                 "1D2247",
                 "B0DACC",
                 "D6CA8B",
                 "E22211",
                 "249991",
                 "E76B6B",
                 "FEFEFE",
                 "42729B",
                 "F6F7F2",
                 "E0E4CC",
                 "D6DAC2",
                 "7BB0A6",
                 "92F22A",
                 "64DDBB",
                 "7CEECE",
                 "8F6F40",
                 "6F532A",
                 "523D1F",
                 "A0B58D",
                 "A19C69",
                 "FD5B03",
                 "63393E",
                 "3C3741",
                 "8C7E51",
                 "54573A",
                 "F04903",
                 "FF7416",
                 "F29B34",
                 "D33257",
                 "3D8EB9",
                 "71BA51",
                 "FEC606",
                 "E75926",
                 "EB6361",
                 "EBBD63",
                 "6C8784",
                 "45362E",
                 "87766C",
                 "25373D",
                 "CF000F",
                 "E3000E",
                 "E6DCDB",
                 "D2D7D3",
                 "E7E7E7",
                 "282830",
                 "BADA55",
                 "1DABB8",
                 "C82647",
                 "FF6766",
                 "60646D",
                 "FFFFF7",
                 "83D6DE",
                 "97CE68",
                 "EB9532",
                 "EE543A",
                 "D8335B",
                 "953163",
                 "422E39",
                 "FACA9B",
                 "FDD09F",
                 "F3D89F",
                 "E7DF86",
                 "C0BA78",
                 "59A9C2",
                 "65878F",
                 "6E5D4B",
                 "6A5A15",
                 "61381B",
                 "4CD4B0",
                 "FFFCE6",
                 "EDD834",
                 "F24D16",
                 "7D1424",
                 "E7E7DE",
                 "CDCBA6",
                 "008891",
                 "00587A",
                 "0F3057",
                 "EFF4E4",
                 "ACA46F",
                 "7574A7",
                 "5659C9",
                 "D4DBC8",
                 "DBD880",
                 "F9AE74",
                 "CD6B97",
                 "557780",
                 "444B54",
                 "8199A3",
                 "B5AFA2",
                 "E1B493",
                 "F7D6B5",
                 "FF9F55",
                 "FF8B55",
                 "FF7E55",
                 "FADAA3",
                 "FF7055",
                 "5C9F97",
                 "DED7E6",
                 "D4AF61",
                 "5B2314",
                 "97B088",
                 "1F9EA3",
                 "F8BD97",
                 "3B0102",
                 "9E5428",
                 "BFB992",
                 "F0F1F5",
                 "112233",
                 "66CC99",
                 "44BBFF",
                 "FC575E",
                 "34495E",
                 "2980B9",
                 "27AE60",
                 "E67E22",
                 "ECF0F1",
                 "E6567A",
                 "BF4A67",
                 "3B3C3D",
                 "47C9AF",
                 "44B39D",
                 "462446",
                 "B05F6D",
                 "EB6B56",
                 "FFC153",
                 "47B39D",
                 "F7E999",
                 "D3EBB2",
                 "5B4B27",
                 "A42A15",
                 "D3E9BA",
                 "F2F8EA",
                 "6797A1",
                 "FABFA1",
                 "E3E7B1",
                 "ECEFA9",
                 "D1CD8E",
                 "BE8B5C",
                 "B86A54",
                 "BA5445",
                 "9E3E25",
                 "BADEB2",
                 "87E8C6",
                 "8BCBDE",
                 "8FA8F6",
                 "B0A4BE",
                 "203040",
                 "E7F76D",
                 "D1D6A9",
                 "EAF2BB",
                 "F7BC05",
                 "53DF83",
                 "47D2E9",
                 "EEEEEE",
                 "3F3F3F",
                 "D1D5D8",
                 "3498DB",
                 "F1C40F",
                 "E74C3C",
                 "1ABC9C",
                 "6D4B11",
                 "563D28",
                 "3F303F",
                 "282256",
                 "11156D",
                 "F4EDF6",
                 "F8D9D5",
                 "D8E2EC",
                 "F2E4F9",
                 "FDE1F7",
                 "1BBC9B",
                 "16A086",
                 "1BA39C",
                 "0B8389",
                 "0F6177",
                 "FCEBB6",
                 "5E412F",
                 "F07818",
                 "F0A830",
                 "78C0A8",
                 "776F70",
                 "E36937",
                 "BBA900",
                 "005057",
                 "E91818",
                 "B8B89F",
                 "DC9855",
                 "FF770B",
                 "816432",
                 "025159",
                 "59AE7F",
                 "64C4AF",
                 "91CED7",
                 "CCEBC0",
                 "D9F5BE",
                 "E3EEFF",
                 "E8FFFF",
                 "E1FAFC",
                 "EDF9FF",
                 "785EDD",
                 "8657DB",
                 "453E4A",
                 "9E58DC",
                 "AE44C8",
                 "9DA5A6",
                 "7FA66C",
                 "48AD01",
                 "C9C1FE",
                 "82B9AD",
                 "7A922D",
                 "722809",
                 "360528",
                 "EE7469",
                 "FFF0D6",
                 "B8959B",
                 "836D6F",
                 "383732",
                 "54ACD2",
                 "5991B1",
                 "5F7187",
                 "48569E",
                 "8B4D93",
                 "4A4E4D",
                 "0E9AA7",
                 "3DA4AB",
                 "F6CD61",
                 "FE8A71",
                 "6BB18C",
                 "ECDAAF",
                 "EBCB94",
                 "EF9688",
                 "DC626F",
                 "FE6860",
                 "FE8A71",
                 "F3C9BF",
                 "D9BBAE",
                 "0C545C",
                 "88F159",
                 "B7EF9C",
                 "F4FFE0",
                 "C9C7AF",
                 "4387B5",
                 "7D7870",
                 "6157D4",
                 "A14C10",
                 "10D2E5",
                 "81E2E6",
                 "93BFB6",
                 "977BAB",
                 "6F2480",
                 "42787A",
                 "409C97",
                 "F8E8B5",
                 "F0340F",
                 "331B17",
                 "34495E",
                 "4F8677",
                 "6B9B61",
                 "8F934D",
                 "B17E22",
                 "413333",
                 "48393C",
                 "744C40",
                 "98583F",
                 "FF7B2C",
                 "4D545E",
                 "586474",
                 "72CCCA",
                 "E2D6BE",
                 "BD3C4E",
                 "CA6769",
                 "F4998A",
                 "F0B799",
                 "F4D6A0",
                 "CDC99F",
                 "528CCB",
                 "F31D2F",
                 "FF8600",
                 "00D717",
                 "BF4ACC",
                 "B3005A",
                 "710301",
                 "8F6910",
                 "F1C40F",
                 "E67E22",
                 "E74C3C",
                 "ECF0F1",
                 "95A5A6",
                 "043D5D",
                 "032E46",
                 "0F595E",
                 "23B684",
                 "FFFFF7",
                 "73B1D6",
                 "4589B0",
                 "1D628B",
                 "444444",
                 "5A5A5A",
                 "F8F8F8",
                 "D6D6D6",
                 "72BAAC",
                 "EE7546",
                 "EE5F5B",
                 "F89406",
                 "FFF457",
                 "62C462",
                 "5BC0DE",
                 "ECF0F1",
                 "1ABC9C",
                 "16A085",
                 "2C3E50",
                 "E74C3C",
                 "7CA39C",
                 "7F8E8B",
                 "4D4D4D",
                 "B27257",
                 "FF5D19",
                 "455869",
                 "3B7E87",
                 "9EA97F",
                 "D1AA7F",
                 "F8BC86",
                 "11132F",
                 "263D4E",
                 "4A6B4E",
                 "918E45",
                 "D9983E",
                 "726680",
                 "FF520F",
                 "0FBAB7",
                 "B8B6A6",
                 "75536C",
                 "181303",
                 "158B93",
                 "031316",
                 "534830",
                 "FD8F04",
                 "BBB7A4",
                 "FFDE49",
                 "FF8F17",
                 "FFFCF5",
                 "FF3209",
                 "1C2236",
                 "08AAC7",
                 "85E2FF",
                 "EEFF6B",
                 "B30802",
                 "02135C",
                 "B7B4B6",
                 "0C1024",
                 "FEDB1D",
                 "E66A39",
                 "D04E33",
                 "353C3E",
                 "1C2021",
                 "EEEEEE",
                 "2C3E50",
                 "34495E",
                 "16A085",
                 "1ABC9C",
                 "BFE6EC",
                 "55C34D",
                 "074354",
                 "053542",
                 "02222B",
                 "22202B",
                 "383745",
                 "7D6962",
                 "CA8D6E",
                 "F9AE74",
                 "1ABC9C",
                 "16A085",
                 "ECF0F1",
                 "E74C3C",
                 "C0392B",
                 "1D1D29",
                 "6C1827",
                 "9E373B",
                 "EF5C54",
                 "FFE3D0",
                 "199EC7",
                 "40BC86",
                 "EC555C",
                 "FCB410",
                 "28A9BC",
                 "36C4D0",
                 "92DBC7",
                 "C6F0DA",
                 "FFE7C1",
                 "292E40",
                 "F7A3A2",
                 "F8CAC1",
                 "BED9DE",
                 "82BBC2",
                 "706F77",
                 "AA8C91",
                 "F1A49F",
                 "E2999F",
                 "E69896",
                 "BFB0A3",
                 "A86E3A",
                 "C4A956",
                 "02000D",
                 "635D4D",
                 "F4F5D6",
                 "E0D5B2",
                 "A38F84",
                 "A38F84",
                 "75706B",
                 "C4C4C4",
                 "706051",
                 "FF7300",
                 "A69688",
                 "84CCD1",
                 "95E7ED",
                 "CC4949",
                 "E35252",
                 "F75959",
                 "9DABA2",
                 "79857E",
                 "27332C",
                 "294543",
                 "2C5957",
                 "131A1E",
                 "1136C7",
                 "1C57E1",
                 "597DF7",
                 "779BF0",
                 "D4C6C6",
                 "9E1616",
                 "37767A",
                 "254F51",
                 "091414",
                 "1E272B"
            };
            Random rnd = new Random();
            int r = rnd.Next(cores.Length);
            return cores[r];
        }
    }
}