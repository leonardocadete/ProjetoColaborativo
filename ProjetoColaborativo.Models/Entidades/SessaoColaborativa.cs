using System;
using System.Collections.Generic;

namespace ProjetoColaborativo.Models.Entidades
{
    public class SessaoColaborativa : Entidade
    {
        public SessaoColaborativa()
        {
            this.ObjetosDaSessao = new List<ObjetoSessao>();
            this.DataCriacao = DateTime.Now;
        }

        public virtual DateTime DataCriacao { get; set; }

        public virtual string Descricao { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual bool Fechada { get; set; }

        public virtual bool Arquivada { get; set; }

        public virtual IList<ObjetoSessao> ObjetosDaSessao { get; set; }

        public virtual IList<Usuario> UsuariosDaSessao { get; set; } 
    }
}
