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

        public virtual string UrlOrigem { get; set; }

        public virtual string UrlImagem { get; set; }

        public virtual string UrlMiniatura { get; set; }

        public virtual DateTime DataCriacao { get; set; }

        public virtual int Ordem { get; set; }

        public virtual IList<ElementoMultimidia> ElementosMultimidia { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
