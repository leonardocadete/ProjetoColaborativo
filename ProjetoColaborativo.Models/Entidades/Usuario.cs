using System.Collections.Generic;
using FluentNHibernate.Testing.Values;

namespace ProjetoColaborativo.Models.Entidades
{
    public class Usuario : Entidade
    {
        public virtual string Nome { get; set; }

        public virtual string Login { get; set; }

        public virtual string Senha { get; set; }

        public virtual string Cpf { get; set; }

        public virtual string Cor { get; set; }

        public virtual IList<ElementoMultimidia> ElementosMultimidia { get; set; }
    }
}
