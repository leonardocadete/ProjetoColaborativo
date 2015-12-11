using System.ComponentModel.DataAnnotations;

namespace ProjetoColaborativo.ViewModels.Account
{
    public class LoginViewModel
    {
        [Display(Name = "Login", Description = "Informar o login da conta")]
        [Required]
        public string Login { get; set; }

        [Display(Name = "Senha", Description = "Informar a senha da conta")]
        [Required]
        public string Senha { get; set; }
    }
}