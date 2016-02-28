using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoColaborativo.Models.Entidades
{
    public class SessaoColaborativa : Entidade
    {
        public SessaoColaborativa()
        {
            this.ListaUsuarios = new List<Usuario>();
            this.ObjetosDaSessao = new List<ObjetoSessao>();
        }

        public virtual DateTime DataCriacao { get; set; }

        public virtual string Descricao { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual IList<Usuario> ListaUsuarios { get; set; }
        public virtual IList<ObjetoSessao> ObjetosDaSessao { get; set; }
    }
}
