using System.Collections.Generic;

namespace ProjetoColaborativo.Models.Entidades
{
    public class Usuario : Entidade
    {

        public Usuario()
        {
            SessoesColaborativas = new List<SessaoColaborativa>();
        }

        public virtual string Nome { get; set; }

        public virtual string Login { get; set; }

        public virtual string Senha { get; set; }

        public virtual string Cpf { get; set; }

        public virtual IList<SessaoColaborativa> SessoesColaborativas { get; set; }
    }
}
