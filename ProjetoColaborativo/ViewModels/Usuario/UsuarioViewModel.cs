using System.ComponentModel.DataAnnotations;
using System.Web;
using ProjetoColaborativo.ValidationAttributes;

namespace ProjetoColaborativo.ViewModels.Usuario
{
    public class UsuarioViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Nome obrigatório")]
        public string Nome { get; set; }

        public string Login { get; set; }

        public string Senha { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Foto { get; set; }

        public string Cor { get; set; }

        [Required(ErrorMessage = "Cpf obrigatório")]
        [Cpf(ErrorMessage = "Cpf inváldo")]
        public string Cpf { get; set; }
    }
}