using System.ComponentModel.DataAnnotations;
using ProjetoColaborativo.ValidationAttributes;

namespace ProjetoColaborativo.ViewModels.Usuario
{
    public class UsuarioViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Nome obrigatório")]
        public string Nome { get; set; }

        public string Login { get; set; }

        public string Senha { get; set; }

        [Required(ErrorMessage = "Cpf obrigatório")]
        [Cpf(ErrorMessage = "Cpf inváldo")]
        public string Cpf { get; set; }
    }
}