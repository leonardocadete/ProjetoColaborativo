using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace ProjetoColaborativo.Models.Entidades
{
    public class Usuario : Entidade, IUser
    {
        public virtual string Nome { get; set; }

        public virtual string Login { get; set; }

        public virtual string Senha { get; set; }

        public virtual string Cpf { get; set; }

        public virtual string Cor { get; set; }

        public virtual IList<ElementoMultimidia> ElementosMultimidia { get; set; }

        public virtual IList<SessaoColaborativa> SessoesColaborativas { get; set; }

        public virtual string Id
        {
            get
            {
                return Handle.ToString();
            }
        }

        public virtual string UserName
        {
            get
            {
                return Login;
            }
            set
            {
                Login = value;
            }
        }

        public override string ToString()
        {
            return this.UserName;
        }

        public virtual int AccessFailedCount { get; set; }
    }
}
