using System;

namespace ProjetoColaborativo.Models.Entidades
{
    public class ElementoMultimidia : Entidade
    {
        public ElementoMultimidia()
        {
            this.DataCriacao = DateTime.Today;
        }

        public virtual Guid Guid { get; set; }
        
        public virtual string Json { get; set; }

        public virtual DateTime DataCriacao { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
