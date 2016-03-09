using System;
using System.Collections.Generic;

namespace ProjetoColaborativo.Models.Entidades
{
    public class ObjetoSessao : Entidade
    {
        public ObjetoSessao()
        {
            ElementosMultimidia = new List<ElementoMultimidia>();
            this.DataCriacao = DateTime.Now;
        }

        public virtual string UrlImagem { get; set; }

        public virtual string UrlMiniatura { get; set; }

        public virtual DateTime DataCriacao { get; set; }

        public virtual IList<ElementoMultimidia> ElementosMultimidia { get; set; }
    }
}
