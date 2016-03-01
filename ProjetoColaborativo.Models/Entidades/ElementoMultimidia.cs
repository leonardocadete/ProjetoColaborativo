using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoColaborativo.Models.Entidades
{
    public class ElementoMultimidia : Entidade
    {
        public virtual Guid Guid { get; set; }

        public virtual ObjetoSessao ObjetoSessao { get; set; }

        public virtual string Json { get; set; }

        public virtual DateTime DataCriacao { get; set; }
    }
}
