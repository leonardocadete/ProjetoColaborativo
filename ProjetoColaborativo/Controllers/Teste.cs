using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetoColaborativo.Controllers
{
    public class Teste : ITeste
    {
        public void Testar()
        {
            System.Diagnostics.Debug.Write("Sabe maluko");
        }
    }
}