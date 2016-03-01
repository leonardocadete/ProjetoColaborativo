using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoColaborativo.Models.Entidades
{
    public class ObjetoSessao : Entidade
    {
        public ObjetoSessao()
        {
            ElementosMultimidia = new List<ElementoMultimidia>();
        }

        public virtual string UrlImagem { get; set; }
        public virtual DateTime DataCriacao { get; set; }

        public virtual SessaoColaborativa SessaoColaborativa { get; set; }

        public virtual IList<ElementoMultimidia> ElementosMultimidia { get; set; }
    }
}
